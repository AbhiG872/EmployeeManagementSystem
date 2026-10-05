using System.ComponentModel.DataAnnotations;

namespace EMS.Web.Models
{
    public class CreateEmployeeLoginViewModel
    {
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}