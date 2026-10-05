using EMS.Entities.Entities;
using EMS.Web.Models;
using EMS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EmailSender _emailSender;


        public AccountController(EmailSender emailSender,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Employee"))
                {
                    return RedirectToAction("MyProfile", "Employee");
                }

                return RedirectToAction("Index", "Employee");
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.MobileNumber
            };

            var result = await _userManager.CreateAsync(
                user,
                model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(
                    user,
                    isPersistent: false);

                TempData["SuccessMessage"] = "Registration successful!";

                return RedirectToAction("Index", "Employee");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Employee"))
                {
                    return RedirectToAction("MyProfile", "Employee");
                }

                return RedirectToAction("Index", "Employee");
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password.");

                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Login successful!";

                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Employee"))
                {
                    return RedirectToAction("MyProfile", "Employee");
                }

                return RedirectToAction("Index", "Employee");
            }

            ModelState.AddModelError(
                string.Empty,
                "Invalid email or password.");

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            TempData["SuccessMessage"] = "Logout successful!";

            return RedirectToAction("Login", "Account");
        }


        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);

                TempData["SuccessMessage"] = "Password changed successfully!";

                return RedirectToAction("Index", "Employee");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
           ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                TempData["SuccessMessage"] =
                    "If the email exists, a password reset link has been sent.";

                return RedirectToAction("ForgotPassword");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action(
                "ResetPassword",
                "Account",
                new
                {
                    email = model.Email,
                    token = token
                },
                Request.Scheme);

            var emailBody = $@"
        <h2>Password Reset</h2>

        <p>Hello {user.FirstName},</p>

        <p>
            We received a request to reset your EMS account password.
        </p>

        <p>
            Click the button below to reset your password:
        </p>

        <p>
            <a href='{resetLink}'
               style='background:#0d6efd;
                      color:white;
                      padding:10px 20px;
                      text-decoration:none;
                      border-radius:5px;'>
                Reset Password
            </a>
        </p>  

        <p>
            If you did not request this, you can ignore this email.
        </p>

        <p>Regards,<br />EMS Team</p>
    ";

            await _emailSender.SendEmailAsync(
                model.Email,
                "EMS - Password Reset",
                emailBody);

            TempData["SuccessMessage"] =
                "Password reset link has been sent to your email.";

            return RedirectToAction("ForgotPassword");
        }
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid password reset request.";

                return RedirectToAction("ForgotPassword");
            }

            var result = await _userManager.ResetPasswordAsync(
                user,
                model.Token,
                model.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] =
                    "Password reset successfully! Please login.";

                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> MakeAdmin()
        {
            var email = "gautampit99@gmail.com";

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Content("User not found.");
            }

            var result = await _userManager.AddToRoleAsync(user, "Admin");

            if (result.Succeeded)
            {
                return Content("Admin role assigned successfully.");
            }

            return Content(string.Join(", ",
                result.Errors.Select(e => e.Description)));
        }
    }
}