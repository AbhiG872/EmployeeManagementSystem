using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EMS.Web.Models
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [Required]
        public string SelectedRole { get; set; } = string.Empty;

        public List<SelectListItem> Roles { get; set; }
            = new List<SelectListItem>();
    }
}