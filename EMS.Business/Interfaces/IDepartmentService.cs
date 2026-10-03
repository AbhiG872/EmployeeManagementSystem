using EMS.Entities.Common;
using EMS.Entities.Entities;

namespace EMS.Business.Interfaces
{
    public interface IDepartmentService
    {
        Task<PaginatedList<Department>> GetAllAsync(
            string? search = null,
            string? sortColumn = null,
            string? sortOrder = null,
            int pageNumber = 1,
            int pageSize = 10);

        Task<Department?> GetByIdAsync(int id);

        Task AddAsync(Department department);

        Task UpdateAsync(Department department);

        Task<bool> DeleteAsync(int id);

        Task<List<Department>> GetLookupAsync();
    }
}