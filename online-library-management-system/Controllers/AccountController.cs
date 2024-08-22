using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using online_library_management_system.Models;
using online_library_management_system.ViewModels;

namespace online_library_management_system.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly UserManager<ApplicationUser> userManager;


    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
    }

    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            //login
            var result = await signInManager.PasswordSignInAsync(model.Email!, model.Password!, model.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await userManager.FindByEmailAsync(model.Email!);
                var role = await userManager.GetRolesAsync(user!);
                if (role.Contains("Admin"))
                {
                    return RedirectToLocal("/Admin");
                }

                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError("", "Invalid login attempt");
        }
        return View(model);
    }

    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            ApplicationUser user = new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address
            };

            var result = await userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user,"user");
                await signInManager.SignInAsync(user, false);

                return RedirectToLocal(returnUrl);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction("Index", "Home");
    }

}
