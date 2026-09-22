using RoleManagementBackend.Application.DTOs;

namespace RoleManagementBackend.Application.Interfaces;

public interface IAuditLogService
{
    Task<List<AuditLogDto>> GetAllAsync();

    Task<AuditLogDto?> GetByIdAsync(int id);

    Task<List<AuditLogDto>> GetByUserIdAsync(int userId);
}