using EMS.Business.Interfaces;
using EMS.DataAccess.Interfaces;
using EMS.Entities.Common;
using EMS.Entities.Entities;

namespace EMS.Business.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(
            IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<PaginatedList<Department>> GetAllAsync(
            string? search = null,
            string? sortColumn = null,
            string? sortOrder = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return await _departmentRepository.GetAllAsync(
                search,
                sortColumn,
                sortOrder,
                pageNumber,
                pageSize);
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Invalid Department Id",
                    nameof(id));
            }

            return await _departmentRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Department department)
        {
            if (department == null)
            {
                return;
            }

            await _departmentRepository.AddAsync(department);
        }

        public async Task UpdateAsync(Department department)
        {
            if (department == null)
            {
                return;
            }

            if (department.DepartmentId <= 0)
            {
                return;
            }

            await _departmentRepository.UpdateAsync(department);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return false;
            }

            var department =
                await _departmentRepository.GetByIdAsync(id);

            if (department == null)
            {
                return false;
            }

            try
            {
                await _departmentRepository.DeleteAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Department>> GetLookupAsync()
        {
            var departments = await _departmentRepository.GetAllAsync(
                pageNumber: 1,
                pageSize: 1000);

            return departments.Items.ToList();
        }
    }
}