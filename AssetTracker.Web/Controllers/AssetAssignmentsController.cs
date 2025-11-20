using AssetTracker.Core.Models;
using AssetTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AssetTracker.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AssetAssignmentsController : Controller
    {
        private readonly AssetTrackerDbContext _context;

        public AssetAssignmentsController(AssetTrackerDbContext context)
        {
            _context = context;
        }

        // GET: AssetAssignments
        public async Task<IActionResult> Index()
        {
            var assignments = await _context.AssetAssignments
                .Include(a => a.Asset)
                .Include(a => a.Employee)
                .ToListAsync();

            return View(assignments);
        }

        // GET: AssetAssignments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var assignment = await _context.AssetAssignments
                .Include(a => a.Asset)
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AssetAssignmentId == id);

            if (assignment == null)
                return NotFound();

            return View(assignment);
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] =
                new SelectList(_context.Employees, "EmployeeId", "Name");

            var availableAssets = _context.Assets
                .Where(a => a.Status == "Available")
                .Select(a => new
                {
                    a.AssetId,
                    DisplayText = $"{a.AssetType} - {a.Brand} {a.Model} (SN: {a.SerialNumber})"
                })
                .ToList();

            ViewData["AssetId"] = new SelectList(availableAssets, "AssetId", "DisplayText");

            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssetAssignment assetAssignment)
        {
            // Remove navigation properties from validation
            ModelState.Remove("Asset");
            ModelState.Remove("Employee");

            if (ModelState.IsValid)
            {
                assetAssignment.AssignedDate = DateTime.Now;

                _context.Add(assetAssignment);

                // Update asset status
                var asset = await _context.Assets.FindAsync(assetAssignment.AssetId);
                if (asset != null)
                    asset.Status = "Assigned";

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EmployeeId"] =
                new SelectList(_context.Employees, "EmployeeId", "Name", assetAssignment.EmployeeId);

            var availableAssets = _context.Assets
                .Where(a => a.Status == "Available" || a.AssetId == assetAssignment.AssetId) // Include currently selected even if assigned (in case of error)
                .Select(a => new
                {
                    a.AssetId,
                    DisplayText = $"{a.AssetType} - {a.Brand} {a.Model} (SN: {a.SerialNumber})"
                })
                .ToList();

            ViewData["AssetId"] = new SelectList(availableAssets, "AssetId", "DisplayText", assetAssignment.AssetId);

            return View(assetAssignment);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var assignment = await _context.AssetAssignments
                .Include(a => a.Asset)
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AssetAssignmentId == id);

            if (assignment == null)
                return NotFound();

            ViewData["EmployeeId"] =
                new SelectList(_context.Employees, "EmployeeId", "Name", assignment.EmployeeId);

            return View(assignment);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AssetAssignment assignment)
        {
            if (id != assignment.AssetAssignmentId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(assignment);

                // If returned, set asset as available
                if (assignment.ReturnedDate != null)
                {
                    var asset = await _context.Assets.FindAsync(assignment.AssetId);
                    if (asset != null)
                        asset.Status = "Available";
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(assignment);
        }

        // GET: Return
        public async Task<IActionResult> Return(int? id)
        {
            if (id == null)
                return NotFound();

            var assignment = await _context.AssetAssignments
                .Include(a => a.Asset)
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AssetAssignmentId == id);

            if (assignment == null)
                return NotFound();

            return View(assignment);
        }

        // POST: Return Confirm
        [HttpPost, ActionName("Return")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnConfirmed(int id)
        {
            var assignment = await _context.AssetAssignments.FindAsync(id);
            if (assignment == null)
                return NotFound();

            assignment.ReturnedDate = DateTime.Now;

            var asset = await _context.Assets.FindAsync(assignment.AssetId);
            if (asset != null)
                asset.Status = "Available";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignment = await _context.AssetAssignments.FindAsync(id);

            if (assignment != null)
                _context.AssetAssignments.Remove(assignment);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
