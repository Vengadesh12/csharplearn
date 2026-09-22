using Microsoft.EntityFrameworkCore;
using RoleManagementBackend.Application.Interfaces;
using RoleManagementBackend.Infrastructure.Persistence;
using RoleManagementBackend.Infrastructure.Persistence.Repositories;
using RoleManagementBackend.Infrastructure.Security;

namespace RoleManagementBackend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=Test;";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();

        return services;
    }
}
