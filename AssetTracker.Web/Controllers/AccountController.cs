using Microsoft.AspNetCore.Mvc;

namespace AssetTracker.Web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
