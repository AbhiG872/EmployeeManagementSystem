using EMS.Business.Interfaces;
using EMS.Entities.Common;
using EMS.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Web.Controllers
{
    [Authorize]
    public class DesignationController : Controller
    {
        private readonly IDesignationService _designationService;
        private readonly IDepartmentService _departmentService;

        public DesignationController(
            IDesignationService designationService,
            IDepartmentService departmentService)
        {
            _designationService = designationService;
            _departmentService = departmentService;
        }


        // GET: Designation
        public async Task<IActionResult> Index(
            string? search,
            int? departmentId,
            string? sortColumn,
            string? sortOrder,
            int pageNumber = 1)
        {
            int pageSize = 10;

            var designations = await _designationService.GetAllAsync(
                search,
                departmentId,
                sortColumn,
                sortOrder,
                pageNumber,
                pageSize);

            await PopulateDepartmentsAsync(departmentId);

            ViewBag.Search = search;
            ViewBag.DepartmentId = departmentId;
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;

            return View(designations);
        }


        // GET: Designation/Create
        [Authorize(Roles = "Admin,HR")]

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDepartmentsAsync();

            return View(new Designation());
        }


        // POST: Designation/Create
        [Authorize(Roles = "Admin,HR")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Designation designation)
        {
            if (ModelState.IsValid)
            {
                await PopulateDepartmentsAsync(designation.DepartmentId);
                return View(designation);
            }

            await _designationService.AddAsync(designation);

            TempData["SuccessMessage"] = "Designation created successfully.";

            return RedirectToAction(nameof(Index));
        }


        // GET: Designation/Edit/5
        [Authorize(Roles = "Admin,HR")]

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var designation =
                await _designationService.GetByIdAsync(id);

            if (designation == null)
            {
                return NotFound();
            }

            await PopulateDepartmentsAsync(
                designation.DepartmentId);

            return View(designation);
        }


        // POST: Designation/Edit/5
        [Authorize(Roles = "Admin,HR")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Designation designation)
        {
            if (id != designation.DesignationId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await PopulateDepartmentsAsync(
                    designation.DepartmentId);

                return View(designation);
            }

            await _designationService.UpdateAsync(designation);

            TempData["SuccessMessage"] =
                "Designation updated successfully.";

            return RedirectToAction(nameof(Index));
        }


        // GET: Designation/Delete/5
        [Authorize(Roles = "Admin")]

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var designation =
                await _designationService.GetByIdAsync(id);

            if (designation == null)
            {
                return NotFound();
            }

            return View(designation);
        }


        // POST: Designation/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted =
                await _designationService.DeleteAsync(id);

            if (deleted)
            {
                TempData["SuccessMessage"] =
                    "Designation deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] =
                    "Designation cannot be deleted because it is linked to employees.";
            }

            return RedirectToAction(nameof(Index));
        }


        // Populate Department Dropdown
        private async Task PopulateDepartmentsAsync(
            int? selectedId = null)
        {
            var departments =
                await _departmentService.GetLookupAsync();

            ViewBag.Departments = new SelectList(
                departments,
                "DepartmentId",
                "DepartmentName",
                selectedId);
        }
    }
}