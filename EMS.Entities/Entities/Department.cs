using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Entities.Entities
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;
        public ICollection<Employee> Employees { get; set; }
            = new List<Employee>();

        public ICollection<Designation> Designations { get; set; }
            = new List<Designation>();

    }
}
