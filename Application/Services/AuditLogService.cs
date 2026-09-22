using RoleManagementBackend.Application.DTOs;
using RoleManagementBackend.Application.Interfaces;
using RoleManagementBackend.Domain.Entities;

namespace RoleManagementBackend.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repository;

    public AuditLogService(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AuditLogDto>> GetAllAsync()
    {
        var logs = await _repository.GetAllAsync();
        return logs.Select(MapToDto).ToList();
    }

    public async Task<AuditLogDto?> GetByIdAsync(int id)
    {
        var log = await _repository.GetByIdAsync(id);
        return log == null ? null : MapToDto(log);
    }

    public async Task<List<AuditLogDto>> GetByUserIdAsync(int userId)
    {
        var logs = await _repository.GetByUserIdAsync(userId);
        return logs.Select(MapToDto).ToList();
    }

    private static AuditLogDto MapToDto(AuditLog log)
    {
        return new AuditLogDto
        {
            Id = log.Id,
            UserId = log.UserId,
            UserName = log.User?.Name,
            UserEmail = log.User?.Email,
            Action = log.Action,
            EntityType = log.EntityType,
            EntityId = log.EntityId,
            OldValues = log.OldValues,
            NewValues = log.NewValues,
            Timestamp = log.Timestamp,
            IpAddress = log.IpAddress,
            UserAgent = log.UserAgent
        };
    }
}