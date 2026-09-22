using RoleManagementBackend.Application.DTOs;
using RoleManagementBackend.Domain.Entities;

namespace RoleManagementBackend.Application.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetUsersAsync();

    Task<User?> GetByIdAsync(int id);

    Task<User?> GetByEmailAsync(string email);

    Task<List<User>> SearchByNameAsync(string name);

    Task<User?> GetUserWithAuditLogsAsync(int id);

    Task<UserAuthDetails?> GetAuthDetailsByEmailAsync(string email);
}