using Microsoft.Extensions.DependencyInjection;
using RoleManagementBackend.Application.Interfaces;
using RoleManagementBackend.Application.Services;

namespace RoleManagementBackend.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
