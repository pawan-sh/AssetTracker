using AssetTracker.Core.Models;
using AssetTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class AssetRequestController : Controller
{
    private readonly AssetTrackerDbContext _ctx;

    public AssetRequestController(AssetTrackerDbContext ctx)
    {
        _ctx = ctx;
    }

    // Employee: Create Asset Request (GET)
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // Employee: Create Asset Request (POST)
    [HttpPost]
    public IActionResult Create(AssetRequest request)
    {
        request.RequestedBy = User.Identity?.Name ?? "Unknown";
        request.RequestedDate = DateTime.Now;
        request.Status = "Pending";

        _ctx.AssetRequests.Add(request);
        _ctx.SaveChanges();

        TempData["Success"] = "Asset request submitted successfully!";
        return RedirectToAction("MyRequests");
    }

    // Employee: My Requests
    public IActionResult MyRequests()
    {
        var userEmail = User.Identity?.Name ?? "";
        var requests = _ctx.AssetRequests
            .Where(r => r.RequestedBy == userEmail)
            .OrderByDescending(r => r.RequestedDate)
            .ToList();

        return View(requests);
    }

    // Admin: List All Requests
    [Authorize(Roles = "Admin")]
    public IActionResult List()
    {
        var requests = _ctx.AssetRequests
            .OrderByDescending(r => r.RequestedDate)
            .ToList();

        return View(requests);
    }

    // Admin: Approve Request
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Approve(int id, string? comments)
    {
        var request = _ctx.AssetRequests.Find(id);
        if (request == null) return NotFound();

        request.Status = "Approved";
        request.AdminComments = comments;
        _ctx.SaveChanges();

        return RedirectToAction("List");
    }

    // Admin: Reject Request
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Reject(int id, string? comments)
    {
        var request = _ctx.AssetRequests.Find(id);
        if (request == null) return NotFound();

        request.Status = "Rejected";
        request.AdminComments = comments;
        _ctx.SaveChanges();

        return RedirectToAction("List");
    }
}
