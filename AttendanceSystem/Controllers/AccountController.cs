using AttendanceSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: true, lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public IActionResult GoogleLogin(string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(GoogleResponse), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse(string? returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["ErrorMessage"] = "Error loading external login information.";
                return RedirectToAction(nameof(Login));
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);
            
            if (signInResult.Succeeded)
            {
                // User already exists with this Google account
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // Auto-create student account for Google logins
                var email = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var firstName = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value ?? "";
                var lastName = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value ?? "";

                if (string.IsNullOrEmpty(email))
                {
                    TempData["ErrorMessage"] = "Unable to get email from Google account.";
                    return RedirectToAction(nameof(Login));
                }

                // Check if user already exists with this email
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    // Link Google login to existing account
                    var addLoginResult = await _userManager.AddLoginAsync(existingUser, info);
                    if (addLoginResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(existingUser, isPersistent: true);
                        TempData["SuccessMessage"] = "Google account linked successfully!";
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to link Google account to existing user.";
                        return RedirectToAction(nameof(Login));
                    }
                }

                // Create new user
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user);
                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Student");
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    TempData["SuccessMessage"] = $"Welcome {firstName}! Your student account has been created.";
                    return RedirectToAction("Index", "Student");
                }
                else
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    TempData["ErrorMessage"] = $"Failed to create account: {errors}";
                    return RedirectToAction(nameof(Login));
                }
            }
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
