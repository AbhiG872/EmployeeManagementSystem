using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [Required(ErrorMessage = "Department is required.")]
        [StringLength(50)]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Designation is required.")]
        [StringLength(50)]
        public string Designation { get; set; } = string.Empty;

        [Range(0, 10000000, ErrorMessage = "Salary must be between 0 and 1,00,0000.")]
        public decimal Salary { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }


        //[Required(ErrorMessage = "Date of Birth is required.")]
        //public DateTime DateOfBirth { get; set; }

        public string? ImagePath { get; set; }
    }
}
