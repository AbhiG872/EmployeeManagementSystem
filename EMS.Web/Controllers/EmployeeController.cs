using EMS.Business.Interfaces;
using EMS.Entities.Entities;
using EMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EMS.Web.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IDesignationService _designationService;

        public EmployeeController(UserManager<ApplicationUser> userManager, IEmployeeService employeeService, IDepartmentService departmentService,
             IDesignationService designationService)
        {
            _userManager = userManager;
            _employeeService = employeeService;
            _departmentService = departmentService;
            _designationService = designationService;
        }
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Index(string? search, string? department, string? status,

            string? sortColumn, string? sortOrder, int pageNumber = 1)

        {
            int pageSize = 10;

            var employees = await _employeeService.
                GetAllAsync(search, department, status, sortColumn, sortOrder, pageNumber, pageSize);


            ViewBag.Search = search;
            ViewBag.Department = department;
            ViewBag.Status = status;
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;

            return View(employees);
        }

        // READ - Employee Details
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployees(
           string? search,
           string? department,
           string? status,
           string? sortColumn,
           string? sortOrder,
           int pageNumber = 1)
        {
            int pageSize = 10;

            var employees = await _employeeService.GetAllAsync(
                search,
                department,
                status,
                sortColumn,
                sortOrder,
                pageNumber,
                pageSize);

            var items = employees.Items.Select(e => new
            {
                employeeId = e.EmployeeId,
                employeeCode = e.EmployeeCode,
                firstName = e.FirstName,
                lastName = e.LastName,
                email = e.Email,
                phone = e.Phone,

                department = e.Department?.DepartmentName,

                designation = e.Designation?.DesignationName,
                reportingManager = e.ReportingManager != null
                ? $"{e.ReportingManager.FirstName} {e.ReportingManager.LastName}"
                : "Not Assigned",
                salary = e.Salary,
                status = e.Status,
                imagePath = e.ImagePath
            });

            return Json(new
            {
                items = items,
                currentPage = employees.CurrentPage,
                pageSize = employees.PageSize,
                totalPages = employees.TotalPages,
                totalRecords = employees.TotalRecords
            });
        }
        private async Task PopulateCreateDropdownsAsync(EmployeeViewModel model)
        {
            // Departments
            var departments = await _departmentService.GetLookupAsync();

            model.Departments = departments
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmentName,
                    Selected = d.DepartmentId == model.DepartmentId
                })
                .ToList();


            // Designations
            if (model.DepartmentId > 0)
            {
                var designations =
                    await _designationService.GetByDepartmentIdAsync(model.DepartmentId);

                model.Designations = designations
                    .Select(d => new SelectListItem
                    {
                        Value = d.DesignationId.ToString(),
                        Text = d.DesignationName,
                        Selected = d.DesignationId == model.DesignationId
                    })
                    .ToList();
            }


            // Reporting Managers
            var managers =
                await _employeeService.GetReportingManagersAsync(model.EmployeeId);

            model.ReportingManagers = managers
                .Select(e => new SelectListItem
                {
                    Value = e.EmployeeId.ToString(),
                    Text = $"{e.FirstName} {e.LastName}",
                    Selected = e.EmployeeId == model.ReportingManagerId
                })
                .ToList();
        }
        // CREATE - GET
        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new EmployeeViewModel
            {
                DateOfJoining = DateTime.Today,
                Status = "Active"
            };

            await PopulateCreateDropdownsAsync(model);

            return View(model);
        }

        // CREATE - POST

        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
                  EmployeeViewModel model,
              IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCreateDropdownsAsync(model);

                return View(model);
            }


            // Create Entity
            var employee = new Employee
            {
                EmployeeCode =
                    await _employeeService.GenerateEmployeeCodeAsync(),

                FirstName = model.FirstName,

                LastName = model.LastName,

                Email = model.Email,

                Phone = model.Phone,

                DateOfJoining = model.DateOfJoining,

                DepartmentId = model.DepartmentId,

                DesignationId = model.DesignationId,

                ReportingManagerId = model.ReportingManagerId,

                Salary = model.Salary,

                Status = model.Status,

                CreatedDate = DateTime.Now
            };


            // Image Upload
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "employees"
                );


                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }


                string uniqueFileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(imageFile.FileName);


                string filePath =
                    Path.Combine(uploadsFolder, uniqueFileName);


                using (var fileStream =
                       new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }


                employee.ImagePath =
                    "/uploads/employees/" + uniqueFileName;
            }


            await _employeeService.AddAsync(employee);

            TempData["SuccessMessage"] =
                "Employee created successfully.";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetDesignations(int departmentId)
        {
            if (departmentId <= 0)
            {
                return Json(new List<object>());
            }

            var designations =
                await _designationService
                    .GetByDepartmentIdAsync(departmentId);

            var result = designations.Select(x => new
            {
                id = x.DesignationId,
                name = x.DesignationName
            });

            return Json(result);
        }
       
        // UPDATE - GET
        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var model = new EmployeeViewModel
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                DateOfJoining = employee.DateOfJoining,
                DepartmentId = employee.DepartmentId,
                DesignationId = employee.DesignationId,
                ReportingManagerId = employee.ReportingManagerId,
                Salary = employee.Salary,
                Status = employee.Status,
                ImagePath = employee.ImagePath
            };

            await PopulateCreateDropdownsAsync(model);

            return View(model);
        }

        // UPDATE - POST
        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeViewModel model, IFormFile? imageFile)
        {

            if (id != model.EmployeeId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateCreateDropdownsAsync(model);
                return View(model);
            }

            var employee = await _employeeService.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            employee.FirstName = model.FirstName;
            employee.LastName = model.LastName;
            employee.Email = model.Email;
            employee.Phone = model.Phone;
            employee.DateOfJoining = model.DateOfJoining;
            employee.DepartmentId = model.DepartmentId;
            employee.DesignationId = model.DesignationId;
            employee.ReportingManagerId = model.ReportingManagerId;
            employee.Salary = model.Salary;
            employee.Status = model.Status;
            employee.UpdatedDate = DateTime.Now;

            // Image update
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/employees");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString()
                               + Path.GetExtension(imageFile.FileName);

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                employee.ImagePath = "/uploads/employees/" + fileName;
            }

            await _employeeService.UpdateAsync(employee);

            TempData["SuccessMessage"] = "Employee updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        // DELETE - GET
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
        [Authorize(Roles = "Admin")]
        // DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _employeeService.DeleteAsync(id);

            TempData["SuccessMessage"] = "Employee deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> MyProfile()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var employee = await _employeeService
                .GetByApplicationUserIdAsync(userId);

            if (employee == null)
            {
                return NotFound("Employee profile not found.");
            }

            return View(employee);
        }

    }
}