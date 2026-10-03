using EMS.Business.Interfaces;
using EMS.Business.Services;
using EMS.DataAccess.Interfaces;
using EMS.DataAccess.Repositories;

namespace EMS.Web.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // Services
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IDesignationService, DesignationService>();

            // Repositories
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IDesignationRepository, DesignationRepository>();

            return services;
        }
    }
}