using EMS.Business.Interfaces;
using EMS.Entities.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Web.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }


        // GET: Department
        public async Task<IActionResult> Index(
            string? search,
            string? sortColumn,
            string? sortOrder,
            int pageNumber = 1)
        {
            int pageSize = 10;

            var departments = await _departmentService.GetAllAsync(
                search,
                sortColumn,
                sortOrder,
                pageNumber,
                pageSize);

            ViewBag.Search = search;
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;

            return View(departments);
        }


        // GET: Department/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Department());
        }


        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            if (!ModelState.IsValid)
            {
                return View(department);
            }

            await _departmentService.AddAsync(department);

            TempData["SuccessMessage"] =
                "Department created successfully.";

            return RedirectToAction(nameof(Index));
        }


        // GET: Department/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var department =
                await _departmentService.GetByIdAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }


        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Department department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(department);
            }

            await _departmentService.UpdateAsync(department);

            TempData["SuccessMessage"] =
                "Department updated successfully.";

            return RedirectToAction(nameof(Index));
        }


        // GET: Department/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var department =
                await _departmentService.GetByIdAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }


        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _departmentService.DeleteAsync(id);


            if (deleted)
            {
                TempData["SuccessMessage"] =
                    "Department deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] =
                    "Department cannot be deleted because it is linked to employees or designations.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}