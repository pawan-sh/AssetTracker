using System.Diagnostics;
using AssetTracker.Web.Models;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Infrastructure.Data;
using System.Linq;

namespace AssetTracker.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AssetTrackerDbContext _context;

        public HomeController(ILogger<HomeController> logger, AssetTrackerDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Redirect employees to My Assets page
                if (User.IsInRole("Employee") || (User.IsInRole("HR") && !User.IsInRole("Admin")))
                {
                    return RedirectToAction("MyAssets", "Issue");
                }

                // Dashboard Logic - Only for Admin/Director
                var model = new DashboardViewModel
                {
                    TotalAssets = _context.Assets.Count(),
                    AssignedAssets = _context.Assets.Count(a => a.Status == "Assigned"),
                    AvailableAssets = _context.Assets.Count(a => a.Status == "Available"),
                    TotalEmployees = _context.Employees.Count(),
                    PendingIssues = _context.Issues.Count(i => i.Status == "Pending"),
                    CompletedRepairs = _context.Issues.Count(i => i.Status == "Completed")
                };
                return View(model);
            }

            ViewData["NoPadding"] = "no-padding";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
