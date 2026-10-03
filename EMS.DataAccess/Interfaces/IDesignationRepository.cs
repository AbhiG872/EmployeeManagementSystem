using EMS.Entities.Common;
using EMS.Entities.Entities;

namespace EMS.DataAccess.Interfaces
{
    public interface IDesignationRepository
    {
        Task<PaginatedList<Designation>> GetAllAsync(
            string? search = null,
            int? departmentId = null,
            string? sortColumn = null,
            string? sortOrder = null,
            int pageNumber = 1,
            int pageSize = 10);

        Task<Designation?> GetByIdAsync(int id);

        Task<List<Designation>> GetByDepartmentIdAsync(
            int departmentId);

        Task AddAsync(Designation designation);

        Task UpdateAsync(Designation designation);

        Task DeleteAsync(int id);
    }
}