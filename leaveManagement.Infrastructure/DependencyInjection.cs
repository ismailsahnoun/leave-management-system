using leaveManagement.Application.Interfaces;
using leaveManagement.Application.Services;
using leaveManagement.Application.Validators;
using leaveManagement.Domain.Repositories;
using leaveManagement.Infrastructure.Data;
using leaveManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace leaveManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // Register repositories
        services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        // Register services
        services.AddScoped<ILeaveRequestService, LeaveRequestService>();
        
        // Register validators
         services.AddScoped<CreateLeaveRequestValidator>();
         services.AddScoped<UpdateLeaveRequestValidator>();
        return services;
    }
}