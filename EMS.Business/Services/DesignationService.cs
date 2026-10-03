using EMS.Business.Interfaces;
using EMS.DataAccess.Interfaces;
using EMS.Entities.Common;
using EMS.Entities.Entities;

namespace EMS.Business.Services
{
    public class DesignationService : IDesignationService
    {
        private readonly IDesignationRepository _designationRepository;

        public DesignationService(
            IDesignationRepository designationRepository)
        {
            _designationRepository = designationRepository;
        }


        public async Task<PaginatedList<Designation>> GetAllAsync(
            string? search = null,
            int? departmentId = null,
            string? sortColumn = null,
            string? sortOrder = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return await _designationRepository.GetAllAsync(
                search,
                departmentId,
                sortColumn,
                sortOrder,
                pageNumber,
                pageSize);
        }


        public async Task<Designation?> GetByIdAsync(int id)
        {
            return await _designationRepository.GetByIdAsync(id);
        }


        public async Task<List<Designation>> GetByDepartmentIdAsync(
            int departmentId)
        {
            return await _designationRepository
                .GetByDepartmentIdAsync(departmentId);
        }


        public async Task AddAsync(Designation designation)
        {
            if (designation == null)
            {
                return;
            }

            await _designationRepository.AddAsync(designation);
        }


        public async Task UpdateAsync(Designation designation)
        {
            if (designation == null)
            {
                return;
            }

            if (designation.DesignationId <= 0)
            {
                return;
            }

            await _designationRepository.UpdateAsync(designation);
        }


        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return false;
            }

            var designation =
                await _designationRepository.GetByIdAsync(id);

            if (designation == null)
            {
                return false;
            }

            try
            {
                await _designationRepository.DeleteAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}