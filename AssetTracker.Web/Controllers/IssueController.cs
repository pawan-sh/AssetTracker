using AssetTracker.Core.Models;
using AssetTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class IssueController : Controller
{
    private readonly AssetTrackerDbContext _ctx;

    public IssueController(AssetTrackerDbContext ctx)
    {
        _ctx = ctx;
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
        issue.ReportedBy = User.Identity.Name;
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

    // Mark repair as completed
    [HttpPost]
    public IActionResult MarkCompleted(int id, string message)
    {
        var issue = _ctx.Issues.FirstOrDefault(i => i.IssueId == id);

        if (issue == null)
            return NotFound();

        issue.Status = "Completed";
        issue.CompletionMessage = message;

        _ctx.SaveChanges();

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
        var user = User.Identity.Name;

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
