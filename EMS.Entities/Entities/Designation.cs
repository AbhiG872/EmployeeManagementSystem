using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Entities.Entities
{
    public class Designation
    {
        [Key]
        public int DesignationId { get; set; }

        public string DesignationName { get; set; } = string.Empty;

        // Foreign Key
        public int DepartmentId { get; set; }

        // Navigation Property
        public Department Department { get; set; } = null!;

        // Navigation Property
        public ICollection<Employee> Employees { get; set; }
            = new List<Employee>();
    }
}
