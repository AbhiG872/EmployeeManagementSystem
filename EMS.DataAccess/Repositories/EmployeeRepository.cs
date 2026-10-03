using EMS.DataAccess.Data;
using EMS.DataAccess.Interfaces;
using EMS.Entities.Common;
using EMS.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.DataAccess.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

      public async Task<PaginatedList<Employee>> GetAllAsync(string? search = null, string? department = null,
            string? status = null, string? sortColumn = null, string? sortOrder = null,
            int pageNumber = 1, int pageSize = 10)

        {
            var query = _context.Employees.AsNoTracking().Include(e => e.Designation).Include(e => e.Department).AsQueryable();

            // Search

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(e =>
                    e.FirstName.Contains(search) ||
                    e.LastName.Contains(search) ||
                    e.Email.Contains(search) ||
                    e.Phone.Contains(search) ||
                    e.Department.DepartmentName.Contains(search) ||
                    e.Designation.DesignationName.Contains(search));
            }


            // Department Filter

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(e =>
                    e.Department.DepartmentName == department);
            }

            // Status Filter

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(e =>
                    e.Status == status);
            }

            // Sorting

            query = sortColumn?.ToLower() switch
            {
                "employeecode" =>
                    sortOrder == "desc"
                        ? query.OrderByDescending(e => e.EmployeeCode)
                        : query.OrderBy(e => e.EmployeeCode),

                "name" =>
                    sortOrder == "desc"
                        ? query.OrderByDescending(e => e.FirstName)
                        : query.OrderBy(e => e.FirstName),

                "email" =>
                    sortOrder == "desc"
                        ? query.OrderByDescending(e => e.Email)
                        : query.OrderBy(e => e.Email),

                "department" =>
                sortOrder == "desc"

                    ? query.OrderByDescending(e => e.Department.DepartmentName)
                    : query.OrderBy(e => e.Department.DepartmentName),

                "designation" =>
                    sortOrder == "desc"
                        ? query.OrderByDescending(e => e.Designation.DesignationName)
                        : query.OrderBy(e => e.Designation.DesignationName),

                "salary" =>
                    sortOrder == "desc"
                        ? query.OrderByDescending(e => e.Salary)
                        : query.OrderBy(e => e.Salary),

                "status" =>
                    sortOrder == "desc"
                        ? query.OrderByDescending(e => e.Status)
                        : query.OrderBy(e => e.Status),

                _ => query.OrderByDescending(e => e.EmployeeId)
            };


            // Total Records

            var totalRecords = await query.CountAsync();


            // Total Pages

            var totalPages =
                (int)Math.Ceiling(
                    totalRecords / (double)pageSize);


            // Pagination

            var employees = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            // Result

            return new PaginatedList<Employee>
            {
                Items = employees,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalRecords = totalRecords
            };
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
           return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
            
        }

        public async Task AddAsync(Employee employee)
        {
            
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            
            employee.EmployeeCode = $"EMP-{employee.EmployeeId:D6}";

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {

            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employee.EmployeeId);

            if (existingEmployee != null)
            {

                _context.Entry(existingEmployee).CurrentValues.SetValues(employee);

                await _context.SaveChangesAsync();
            }
        }


        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id);


            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<string> GenerateEmployeeCodeAsync()
        {
            var lastEmployee = await _context.Employees
                .OrderByDescending(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastEmployee != null)
            {
                nextNumber = lastEmployee.EmployeeId + 1;
            }

            return $"EMP-{nextNumber:D6}";
        }

    }
}
