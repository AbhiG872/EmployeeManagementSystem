using EMS.DataAccess.Data;
using EMS.DataAccess.Interfaces;
using EMS.Entities.Common;
using EMS.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace EMS.DataAccess.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Department>> GetAllAsync(
            string? search = null,
            string? sortColumn = null,
            string? sortOrder = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Departments
                .AsNoTracking()
                .AsQueryable();

            // Searching
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.DepartmentName.Contains(search));
            }

            // Sorting
            bool isDescending = sortOrder == "desc";

            query = sortColumn?.ToLower() switch
            {
                "departmentname" => isDescending
                    ? query.OrderByDescending(x => x.DepartmentName)
                           .ThenBy(x => x.DepartmentId)
                    : query.OrderBy(x => x.DepartmentName)
                           .ThenBy(x => x.DepartmentId),

                _ => query.OrderBy(x => x.DepartmentId)
            };

            // Pagination
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);

            int totalRecords = await query.CountAsync();

            int totalPages = (int)Math.Ceiling(
                totalRecords / (double)pageSize);

            var departments = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<Department>
            {
                Items = departments,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages
            };
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(x => x.DepartmentId == id);
        }

        public async Task AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Department department)
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var department = await GetByIdAsync(id);

            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
        }
    }
}