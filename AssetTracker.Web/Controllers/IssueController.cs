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
    private readonly EventGridService _eventGrid;
    private readonly IWebHostEnvironment _webHostEnvironment; // <-- new

    public IssueController(AssetTrackerDbContext ctx, EventGridService eventGrid, IWebHostEnvironment webHostEnvironment)
    {
        _ctx = ctx;
        _eventGrid = eventGrid;
        _webHostEnvironment = webHostEnvironment; // <-- new
    }

    // Report Issue (Employee)
    [HttpGet]
    public IActionResult Create(int? assetId)
    {
        if (assetId.HasValue)
        {
            var asset = _ctx.Assets.Find(assetId.Value);
            if (asset != null)
            {
                var model = new Issue
                {
                    AssetId = asset.AssetId,
                    AssetName = $"{asset.AssetType} - {asset.Brand} {asset.Model} ({asset.SerialNumber})"
                };
                return View(model);
            }
        }
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

    // Employee: My Assets
    public IActionResult MyAssets()
    {
        var userEmail = User.Identity?.Name ?? "";

        var assignments = _ctx.AssetAssignments
            .Include(a => a.Asset)
            .Include(a => a.Employee)
            .Where(a => a.Employee != null && a.Employee.Email == userEmail && a.ReturnedDate == null)
            .ToList();

        return View(assignments);
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
    // Admin: Start Internal Repair
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult StartInternalRepair(int id)
    {
        var issue = _ctx.Issues.Find(id);
        if (issue == null) return NotFound();

        issue.Status = "In Progress";
        _ctx.SaveChanges();
        return RedirectToAction("List");
    }

    // Admin: Close Issue (GET)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Close(int id)
    {
        var issue = _ctx.Issues.Find(id);
        if (issue == null) return NotFound();
        return View(issue);
    }

    // Admin: Close Issue (POST)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Close(int id, string resolution, decimal? cost, string invoiceReference, string message, IFormFile? invoiceFile)
    {
        var issue = _ctx.Issues.Find(id);
        if (issue == null) return NotFound();

        issue.Status = "Completed";
        issue.Resolution = resolution;
        issue.Cost = cost;
        issue.InvoiceReference = invoiceReference;
        issue.CompletionMessage = message;

        // Handle File Upload
        if (invoiceFile != null && invoiceFile.Length > 0)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "invoices");
            Directory.CreateDirectory(uploadsFolder); // Ensure directory exists

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + invoiceFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await invoiceFile.CopyToAsync(fileStream);
            }

            issue.InvoicePath = "/uploads/invoices/" + uniqueFileName;
        }

        _ctx.SaveChanges();

        // ⭐ Send event to Event Grid so Logic App can email employee
        await _eventGrid.PublishEventAsync("Asset.RepairCompleted", new
        {
            IssueId = issue.IssueId,
            AssetName = issue.AssetName,
            EmployeeEmail = issue.ReportedBy,
            Message = message,
            Resolution = resolution,
            Cost = cost,
            CompletedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt")
        });

        return RedirectToAction("List");
    }

    //Mark Issue as Completed (Legacy - Redirect to Close)
    [HttpPost]
    public IActionResult MarkCompleted(int id)
    {
        return RedirectToAction("Close", new { id });
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
