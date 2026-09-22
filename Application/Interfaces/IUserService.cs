using RoleManagementBackend.Application.DTOs;

namespace RoleManagementBackend.Application.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetUsersAsync();

    Task<UserDto?> GetByIdAsync(int id);

    Task<List<UserDto>> SearchByNameAsync(string name);

    Task<UserWithAuditLogsDto?> GetUserWithAuditLogsAsync(int id);
}