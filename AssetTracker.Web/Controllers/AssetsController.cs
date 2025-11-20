using AssetTracker.Core.Models;
using AssetTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssetTracker.Web.Controllers
{
    [Authorize(Roles = "Admin,Director")]
    public class AssetsController : Controller
    {
        private readonly AssetTrackerDbContext _context;

        public AssetsController(AssetTrackerDbContext context)
        {
            _context = context;
        }

        // GET: Assets
        public async Task<IActionResult> Index(string searchString)
        {
            var assets = from a in _context.Assets
                         select a;

            if (!string.IsNullOrEmpty(searchString))
            {
                assets = assets.Where(s => s.AssetType.Contains(searchString)
                                       || s.Brand.Contains(searchString)
                                       || s.Model.Contains(searchString)
                                       || (s.SerialNumber != null && s.SerialNumber.Contains(searchString)));
            }

            ViewData["CurrentFilter"] = searchString;
            return View(await assets.ToListAsync());
        }

        // GET: Assets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var asset = await _context.Assets
                .FirstOrDefaultAsync(m => m.AssetId == id);

            if (asset == null)
                return NotFound();

            return View(asset);
        }

        // GET: Assets/Create
        public IActionResult Create()
        {
            ViewBag.AssetTypes = new List<string> { "Laptop", "Desktop", "Monitor", "Phone", "Tablet", "Printer", "Accessory", "Other" };
            ViewBag.Statuses = new List<string> { "Available", "In Use", "Broken", "In Repair", "Retired" };
            return View();
        }

        // POST: Assets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Asset asset)
        {
            if (ModelState.IsValid)
            {
                _context.Add(asset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.AssetTypes = new List<string> { "Laptop", "Desktop", "Monitor", "Phone", "Tablet", "Printer", "Accessory", "Other" };
            ViewBag.Statuses = new List<string> { "Available", "In Use", "Broken", "In Repair", "Retired" };
            return View(asset);
        }

        // GET: Assets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
                return NotFound();

            return View(asset);
        }

        // POST: Assets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Asset asset)
        {
            if (id != asset.AssetId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Assets.Any(e => e.AssetId == asset.AssetId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(asset);
        }

        // GET: Assets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var asset = await _context.Assets
                .FirstOrDefaultAsync(m => m.AssetId == id);

            if (asset == null)
                return NotFound();

            return View(asset);
        }

        // POST: Assets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset != null)
            {
                _context.Assets.Remove(asset);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
