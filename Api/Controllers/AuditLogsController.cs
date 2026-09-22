using Microsoft.AspNetCore.Mvc;
using RoleManagementBackend.Application.Common;
using RoleManagementBackend.Application.DTOs;
using RoleManagementBackend.Application.Interfaces;

namespace RoleManagementBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    [HttpGet("/api/entityframework/auditlogs")]
    [HttpGet("/api/audit/all")]
    public async Task<IActionResult> GetAllAuditLogs()
    {
        var logs = await _auditLogService.GetAllAsync();
        return Ok(ApiResponse<List<AuditLogDto>>.Ok(logs));
    }

    [HttpGet("{id:int}")]
    [HttpGet("/api/check/audit/{id:int}")]
    public async Task<IActionResult> GetAuditLogById(int id)
    {
        var log = await _auditLogService.GetByIdAsync(id);
        if (log == null)
        {
            return NotFound(ApiResponse<AuditLogDto>.Fail($"Audit log with ID {id} was not found."));
        }

        return Ok(ApiResponse<AuditLogDto>.Ok(log));
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetAuditLogsByUserId(int userId)
    {
        var logs = await _auditLogService.GetByUserIdAsync(userId);
        return Ok(ApiResponse<List<AuditLogDto>>.Ok(logs));
    }
}
