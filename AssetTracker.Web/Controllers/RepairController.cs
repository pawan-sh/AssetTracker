using AssetTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class RepairController : Controller
{
    private readonly AssetTrackerDbContext _ctx;

    public RepairController(AssetTrackerDbContext ctx)
    {
        _ctx = ctx;
    }


    [HttpGet]
    public IActionResult Create(int issueId)
    {
        ViewBag.IssueId = issueId;
        return View();
    }

    [HttpPost]
    public IActionResult Create(Repair repair)
    {
        repair.RepairDate = DateTime.Now;

        _ctx.Repairs.Add(repair);

        // update issue status
        var issue = _ctx.Issues.FirstOrDefault(i => i.IssueId == repair.IssueId);
        issue.Status = "InRepair";

        _ctx.SaveChanges();

        return RedirectToAction("List", "Issue");
    }

    public IActionResult Edit(int id)
    {
        var repair = _ctx.Repairs.FirstOrDefault(r => r.RepairId == id);

        if (repair == null) return NotFound();

        return View(repair);
    }

    [HttpPost]
    public IActionResult Edit(Repair model)
    {
        if (ModelState.IsValid)
        {
            _ctx.Repairs.Update(model);
            _ctx.SaveChanges();
            return RedirectToAction("Details", new { id = model.IssueId });
        }
        return View(model);
    }

}
