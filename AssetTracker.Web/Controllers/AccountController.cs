using AssetTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AssetTrackerDbContext _context;

    public AccountController(SignInManager<IdentityUser> signInManager,
                             UserManager<IdentityUser> userManager,
                             AssetTrackerDbContext context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
    }

    [Authorize]
    public IActionResult Profile()
    {
        return View();
    }


    // LOGIN (GET)
    public IActionResult Login()
    {
        return View();
    }

    // LOGIN (POST)
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        ViewBag.Error = "Invalid login attempt!";
        return View();
    }

    // EMPLOYEE LOGIN (GET)
    public IActionResult EmployeeLogin()
    {
        return View();
    }

    // EMPLOYEE LOGIN (POST)
    [HttpPost]
    public async Task<IActionResult> EmployeeLogin(string email)
    {
        // 1. Check if Identity User exists
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            // 2. If not, check if they are a valid Employee in the database
            var employeeExists = await _context.Employees.AnyAsync(e => e.Email == email);

            if (employeeExists)
            {
                // 3. Auto-register them as an Identity User
                user = new IdentityUser { UserName = email, Email = email };
                var createResult = await _userManager.CreateAsync(user, email); // Password = Email

                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Employee");
                }
                else
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    ViewBag.Error = $"Error creating account: {errors}";
                    return View();
                }
            }
            else
            {
                ViewBag.Error = "Employee email not found in the system.";
                return View();
            }
        }

        // 4. Sign in
        var result = await _signInManager.PasswordSignInAsync(user, email, false, false);

        if (result.Succeeded)
        {
            return RedirectToAction("MyAssets", "Issue");
        }

        ViewBag.Error = "Login failed. Please ensure your account is set up correctly.";
        return View();
    }

    // REGISTER (GET)
    public IActionResult Register()
    {
        return View();
    }

    // REGISTER (POST)
    [HttpPost]
    public async Task<IActionResult> Register(string email, string password)
    {
        var user = new IdentityUser { UserName = email, Email = email };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            // Default New Users → Employee role
            await _userManager.AddToRoleAsync(user, "Employee");
            return RedirectToAction("Login");
        }

        ViewBag.Error = string.Join(", ", result.Errors.Select(e => e.Description));
        return View();
    }

    // LOGOUT
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
