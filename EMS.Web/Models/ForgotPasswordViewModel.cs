using System.ComponentModel.DataAnnotations;

namespace EMS.Web.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
