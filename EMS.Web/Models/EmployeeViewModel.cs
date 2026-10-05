using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Web.Models
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }

        public string EmployeeCode { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public DateTime DateOfJoining { get; set; }

        public int DepartmentId { get; set; }

        public int DesignationId { get; set; }

        public decimal Salary { get; set; }

        public string Status { get; set; } = "Active";

        public string? ImagePath { get; set; }

        // Reporting Manager
        public int? ReportingManagerId { get; set; }

        // Dropdowns
        public List<SelectListItem> Departments { get; set; }
            = new List<SelectListItem>();

        public List<SelectListItem> Designations { get; set; }
            = new List<SelectListItem>();

        public List<SelectListItem> ReportingManagers { get; set; }
            = new List<SelectListItem>();
    }
}