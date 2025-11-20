using AssetTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Web.Services;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class RepairController : Controller
{
    private readonly AssetTrackerDbContext _ctx;
    private readonly EventGridService _eventGrid;

    public RepairController(AssetTrackerDbContext ctx, EventGridService eventGrid)
    {
        _ctx = ctx;
        _eventGrid = eventGrid;
    }

    // --- GET: Create Repair ---
    [HttpGet]
    public IActionResult Create(int issueId)
    {
        ViewBag.IssueId = issueId;
        return View();
    }

    // --- POST: Create Repair + Send Event ---
    [HttpPost]
    public async Task<IActionResult> Create(Repair repair)
    {
        repair.RepairDate = DateTime.Now;

        _ctx.Repairs.Add(repair);

        // Fetch issue
        var issue = await _ctx.Issues.FirstOrDefaultAsync(i => i.IssueId == repair.IssueId);

        if (issue == null)
            return NotFound();

        // Update issue status
        issue.Status = "InRepair";

        // Save everything
        await _ctx.SaveChangesAsync();

        // 🔥 Send EventGrid Notification (Repair Started)
        await _eventGrid.PublishEventAsync("Asset.RepairStarted", new
        {
            IssueId = issue.IssueId,
            AssetName = issue.AssetName,
            Description = issue.Description,
            EmployeeEmail = issue.ReportedBy,
            TechnicianName = repair.TechnicianName,
            TechnicianPhone = repair.TechnicianPhone,
            Cost = repair.RepairCost,
            StartedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt")
        });

        return RedirectToAction("List", "Issue");
    }

    // --- GET: Edit Repair ---
    public async Task<IActionResult> Edit(int id)
    {
        var repair = await _ctx.Repairs.FirstOrDefaultAsync(r => r.RepairId == id);

        if (repair == null)
            return NotFound();

        return View(repair);
    }

    // --- POST: Edit Repair ---
    [HttpPost]
    public async Task<IActionResult> Edit(Repair model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _ctx.Repairs.Update(model);
        await _ctx.SaveChangesAsync();

        return RedirectToAction("Details", new { id = model.IssueId });
    }
}
