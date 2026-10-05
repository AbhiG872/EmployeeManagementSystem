using EMS.DataAccess.Data;
using EMS.Entities.Entities;
using EMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EMS.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

       
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.ApplicationUser)
                .ToListAsync();

            return View(employees);
        }

        [HttpGet]
        public async Task<IActionResult> AssignRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = _roleManager.Roles
                .Select(r => r.Name!)
                .ToList();

            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new UserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? "",
                SelectedRole = userRoles.FirstOrDefault() ?? "",
                Roles = roles.Select(role => new SelectListItem
                {
                    Text = role,
                    Value = role
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(
            UserRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadRoles(model);
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                var removeResult =
                    await _userManager.RemoveFromRolesAsync(
                        user,
                        currentRoles);

                if (!removeResult.Succeeded)
                {
                    foreach (var error in removeResult.Errors)
                    {
                        ModelState.AddModelError(
                            "",
                            error.Description);
                    }

                    await LoadRoles(model);
                    return View(model);
                }
            }

            var addResult = await _userManager.AddToRoleAsync(
                user,
                model.SelectedRole);

            if (!addResult.Succeeded)
            {
                foreach (var error in addResult.Errors)
                {
                    ModelState.AddModelError(
                        "",
                        error.Description);
                }

                await LoadRoles(model);
                return View(model);
            }

            TempData["SuccessMessage"] =
                $"Role '{model.SelectedRole}' assigned successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadRoles(UserRoleViewModel model)
        {
            model.Roles = await _roleManager.Roles
                .Select(r => new SelectListItem
                {
                    Text = r.Name!,
                    Value = r.Name!
                })
                .ToListAsync();
        }
        [HttpGet]
        public async Task<IActionResult> CreateLoginAccess(int id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(employee.ApplicationUserId))
            {
                TempData["ErrorMessage"] =
                    "Login access already exists for this employee.";

                return RedirectToAction(nameof(Index));
            }

            var model = new CreateEmployeeLoginViewModel
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.FirstName + " " + employee.LastName
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLoginAccess(
    CreateEmployeeLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == model.EmployeeId);

            if (employee == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(employee.ApplicationUserId))
            {
                ModelState.AddModelError(
                    "",
                    "Login access already exists for this employee.");

                return View(model);
            }

            var existingUser = await _userManager
                .FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError(
                    "Email",
                    "This email is already registered.");

                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = employee.FirstName,
                LastName = employee.LastName
            };

            var result = await _userManager
                .CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        "",
                        error.Description);
                }

                return View(model);
            }

            var roleResult = await _userManager
                .AddToRoleAsync(user, "Employee");

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(
                        "",
                        error.Description);
                }

                return View(model);
            }

            employee.ApplicationUserId = user.Id;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Employee login access created successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}