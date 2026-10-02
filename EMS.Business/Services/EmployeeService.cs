using EMS.Business.Interfaces;
using EMS.DataAccess.Interfaces;
using EMS.Entities.Common;
using EMS.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Business.Services
{
    public class EmployeeService:IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<PaginatedList<Employee>> GetAllAsync(
     string? search = null,
     string? department = null,
     string? status = null,
     string? sortColumn = null,
     string? sortOrder = null,
     int pageNumber = 1,
     int pageSize = 10)
        {
            return await _employeeRepository.GetAllAsync(
                search,
                department,
                status,
                sortColumn,
                sortOrder,
                pageNumber,
                pageSize);
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid Employee Id", nameof(id));
            }

            return await _employeeRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Employee employee)
        {
            await _employeeRepository.AddAsync(employee);
        }


        public async Task UpdateAsync(Employee employee)
        {
            if (employee == null)
            {
                return;
            }
            if (employee.EmployeeId <= 0)
            {
                return;
            }
            await _employeeRepository.UpdateAsync(employee);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return;
            }
            await _employeeRepository.DeleteAsync(id);
        }
        public async Task<string> GenerateEmployeeCodeAsync()
        {
            return await _employeeRepository.GenerateEmployeeCodeAsync();
        }
    }
}

