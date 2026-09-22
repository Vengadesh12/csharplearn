using RoleManagementBackend.Domain.Entities;

namespace RoleManagementBackend.Application.Interfaces;

public interface IAuditLogRepository
{
    Task<List<AuditLog>> GetAllAsync();

    Task<AuditLog?> GetByIdAsync(int id);

    Task<List<AuditLog>> GetByUserIdAsync(int userId);

    Task AddAsync(AuditLog log);
}