using EMS.Business.Interfaces;
using EMS.Entities.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Web.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IDesignationService _designationService;

        public EmployeeController(IEmployeeService employeeService, IDepartmentService departmentService,
             IDesignationService designationService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _designationService = designationService;
        }

        // READ - Employee List
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
        private async Task PopulateDepartmentsAsync(int? selectedDepartmentId = null)
        {
            var departments = await _departmentService.GetLookupAsync();


            ViewBag.Departments = new SelectList(departments, "DepartmentId", "DepartmentName",
                selectedDepartmentId);

        }
        // CREATE - GET
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDepartmentsAsync();

            return View(new Employee());
        }

        // CREATE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee, IFormFile? imageFile)
        {
            employee.EmployeeCode = await _employeeService.GenerateEmployeeCodeAsync();

            if (ModelState.IsValid)
            {
                await PopulateDepartmentsAsync(employee.DepartmentId);

                return View(employee);
            }

            employee.CreatedDate = DateTime.Now;

            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot", "uploads", "employees");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(imageFile.FileName);

                string filePath = Path.Combine(
                    uploadsFolder,
                    uniqueFileName);

                using (var fileStream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                employee.ImagePath =
                    "/uploads/employees/" + uniqueFileName;
            }

            await _employeeService.AddAsync(employee);
            TempData["SuccessMessage"] = "Employee created successfully.";

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
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // UPDATE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            employee.UpdatedDate = DateTime.Now;
            await _employeeService.UpdateAsync(employee);
            TempData["SuccessMessage"] = "Employee updated successfully.";
            return RedirectToAction(nameof(Index));
        }
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

        // DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _employeeService.DeleteAsync(id);

            TempData["SuccessMessage"] = "Employee deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
      
    }
}