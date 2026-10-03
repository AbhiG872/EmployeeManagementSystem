using EMS.DataAccess.Data;
using EMS.DataAccess.Interfaces;
using EMS.Entities.Common;
using EMS.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace EMS.DataAccess.Repositories
{
    public class DesignationRepository : IDesignationRepository
    {
        private readonly ApplicationDbContext _context;

        public DesignationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Designation>> GetAllAsync(
            string? search = null,
            int? departmentId = null,
            string? sortColumn = null,
            string? sortOrder = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Designations
                .Include(x => x.Department)
                .AsNoTracking()
                .AsQueryable();

            // Searching
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.DesignationName.Contains(search) ||
                    x.Department.DepartmentName.Contains(search));
            }

            // Department Filtering
            if (departmentId.HasValue && departmentId.Value > 0)
            {
                query = query.Where(x =>
                    x.DepartmentId == departmentId.Value);
            }

            // Sorting
            bool isDescending = sortOrder == "desc";

            query = sortColumn?.ToLower() switch
            {
                "designationname" => isDescending
                    ? query.OrderByDescending(x => x.DesignationName)
                           .ThenBy(x => x.DesignationId)
                    : query.OrderBy(x => x.DesignationName)
                           .ThenBy(x => x.DesignationId),

                "departmentname" => isDescending
                    ? query.OrderByDescending(x =>
                          x.Department.DepartmentName)
                           .ThenBy(x => x.DesignationId)
                    : query.OrderBy(x =>
                          x.Department.DepartmentName)
                           .ThenBy(x => x.DesignationId),

                _ => query.OrderBy(x => x.DesignationId)
            };

            // Pagination
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);

            int totalRecords = await query.CountAsync();

            int totalPages = (int)Math.Ceiling(
                totalRecords / (double)pageSize);

            var designations = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<Designation>
            {
                Items = designations,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages
            };
        }

        public async Task<Designation?> GetByIdAsync(int id)
        {
            return await _context.Designations
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.DesignationId == id);
        }

        public async Task<List<Designation>> GetByDepartmentIdAsync(
            int departmentId)
        {
            return await _context.Designations
                .AsNoTracking()
                .Where(x => x.DepartmentId == departmentId)
                .OrderBy(x => x.DesignationName)
                .ToListAsync();
        }

        public async Task AddAsync(Designation designation)
        {
            await _context.Designations.AddAsync(designation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Designation designation)
        {
            _context.Designations.Update(designation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var designation = await GetByIdAsync(id);

            if (designation != null)
            {
                _context.Designations.Remove(designation);
                await _context.SaveChangesAsync();
            }
        }
    }
}