using System;
using System.ComponentModel.DataAnnotations;

namespace EMS.Entities.Entities
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Employee Code is required.")]
        [StringLength(20)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(15)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Joining is required.")]
        public DateTime DateOfJoining { get; set; }

        // Department
        [Required(ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        public Department Department { get; set; } = null!;

        // Designation
        [Required(ErrorMessage = "Designation is required.")]
        public int DesignationId { get; set; }

        public Designation Designation { get; set; } = null!;

        public decimal Salary { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? ImagePath { get; set; }
        public string? ApplicationUserId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }
    }
}