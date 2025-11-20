using AssetTracker.Core.Models;
using AssetTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetTracker.Web.Services;


[Authorize]
public class IssueController : Controller
{
    private readonly AssetTrackerDbContext _ctx;
    private readonly EventGridService _eventGrid;   // <-- new

    public IssueController(AssetTrackerDbContext ctx, EventGridService eventGrid)
    {
        _ctx = ctx;
        _eventGrid = eventGrid;                    // <-- new
    }

    // Report Issue (Employee)
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Issue issue)
    {
        issue.ReportedDate = DateTime.Now;
        issue.ReportedBy = User.Identity?.Name ?? "Unknown";
        issue.Status = "Pending";

        _ctx.Issues.Add(issue);
        _ctx.SaveChanges();

        return RedirectToAction("MyIssues");
    }

    // View Repair Details
    public IActionResult RepairDetails(int id)
    {
        var issue = _ctx.Issues
            .Include(i => i.Repair)
            .FirstOrDefault(i => i.IssueId == id);

        if (issue == null)
            return NotFound();

        return View(issue);
    }
    //Mark Issue as Completed
    [HttpPost]
    public async Task<IActionResult> MarkCompleted(int id, string message)
    {
        var issue = _ctx.Issues.FirstOrDefault(i => i.IssueId == id);

        if (issue == null)
            return NotFound();

        issue.Status = "Completed";
        issue.CompletionMessage = message;

        _ctx.SaveChanges();

        // ⭐ Send event to Event Grid so Logic App can email employee
        await _eventGrid.PublishEventAsync("Asset.RepairCompleted", new
        {
            IssueId = issue.IssueId,
            AssetName = issue.AssetName,
            EmployeeEmail = issue.ReportedBy,   // you might map email differently
            Message = message,
            CompletedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt")
        });

        return RedirectToAction("List");
    }


    [HttpPost]
    public IActionResult Delete(int id)
    {
        var issue = _ctx.Issues.FirstOrDefault(i => i.IssueId == id);

        if (issue == null)
            return NotFound();

        // Optional: Prevent deleting issues still in repair
        if (issue.Status == "InRepair")
            return BadRequest("Cannot delete an issue that is currently in repair.");

        _ctx.Issues.Remove(issue);
        _ctx.SaveChanges();

        return RedirectToAction("MyIssues");
    }



    // Employee: My Issues
    public IActionResult MyIssues()
    {
        var user = User.Identity?.Name ?? "";

        var issues = _ctx.Issues
            .Include(i => i.Repair)
            .Where(x => x.ReportedBy == user)
            .ToList();

        return View(issues);
    }

    // Admin: All Issues
    [Authorize(Roles = "Admin")]
    public IActionResult List()
    {
        var issues = _ctx.Issues
            .Include(i => i.Repair)
            .OrderByDescending(x => x.IssueId)
            .ToList();

        return View(issues);
    }
}
