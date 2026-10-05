using EMS.Entities.Common;
using EMS.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Business.Interfaces
{
    public interface IEmployeeService
    {
        Task<PaginatedList<Employee>> GetAllAsync(
             string? search = null,
             string? department = null,
             string? status = null,
             string? sortColumn = null,
             string? sortOrder = null,
             int pageNumber = 1,
             int pageSize = 10);
        Task<Employee?> GetByApplicationUserIdAsync(string userId);
        Task<Employee?> GetByIdAsync(int id);

        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);

        Task DeleteAsync(int id);

        Task<string> GenerateEmployeeCodeAsync();
    }
}
