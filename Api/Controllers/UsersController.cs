using Microsoft.AspNetCore.Mvc;
using RoleManagementBackend.Application.Common;
using RoleManagementBackend.Application.DTOs;
using RoleManagementBackend.Application.Interfaces;

namespace RoleManagementBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuditLogService _auditLogService;

    public UsersController(IUserService userService, IAuditLogService auditLogService)
    {
        _userService = userService;
        _auditLogService = auditLogService;
    }

    [HttpGet]
    [HttpGet("/api/entityframework/users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetUsersAsync();
        return Ok(ApiResponse<List<UserDto>>.Ok(users));
    }

    [HttpGet("search")]
    [HttpGet("/api/entityframework/users/search")]
    public async Task<IActionResult> SearchUsers([FromQuery] string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(ApiResponse<List<UserDto>>.Fail("Query parameter 'name' is required."));
        }

        var users = await _userService.SearchByNameAsync(name);
        return Ok(ApiResponse<List<UserDto>>.Ok(users));
    }

    [HttpGet("{id:int}")]
    [HttpGet("/api/entityframework/users/{id:int}")]
    [HttpGet("/api/check/json/{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound(ApiResponse<UserDto>.Fail($"User with ID {id} was not found."));
        }

        return Ok(ApiResponse<UserDto>.Ok(user));
    }

    [HttpGet("{id:int}/auditlogs")]
    [HttpGet("/api/entityframework/users/{id:int}/auditlogs")]
    public async Task<IActionResult> GetAuditLogsForUser(int id)
    {
        var logs = await _auditLogService.GetByUserIdAsync(id);
        return Ok(ApiResponse<List<AuditLogDto>>.Ok(logs));
    }

    [HttpGet("{id:int}/with-auditlogs")]
    [HttpGet("/api/entityframework/users/{id:int}/with-auditlogs")]
    public async Task<IActionResult> GetUserWithAuditLogs(int id)
    {
        var user = await _userService.GetUserWithAuditLogsAsync(id);
        if (user == null)
        {
            return NotFound(ApiResponse<UserWithAuditLogsDto>.Fail($"User with ID {id} was not found."));
        }

        return Ok(ApiResponse<UserWithAuditLogsDto>.Ok(user));
    }
}