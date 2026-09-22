using RoleManagementBackend.Application.DTOs;
using RoleManagementBackend.Application.Interfaces;
using RoleManagementBackend.Domain.Entities;

namespace RoleManagementBackend.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        var users = await _userRepository.GetUsersAsync();
        return users.Select(MapToDto).ToList();
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<List<UserDto>> SearchByNameAsync(string name)
    {
        var users = await _userRepository.SearchByNameAsync(name);
        return users.Select(MapToDto).ToList();
    }

    public async Task<UserWithAuditLogsDto?> GetUserWithAuditLogsAsync(int id)
    {
        var user = await _userRepository.GetUserWithAuditLogsAsync(id);
        if (user == null)
        {
            return null;
        }

        return new UserWithAuditLogsDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Age = user.Age,
            Address = user.Address,
            RoleId = user.RoleId,
            RoleName = user.Role?.Name,
            DeletedFlag = user.DeletedFlag,
            DesignationId = user.DesignationId,
            DesignationName = user.Designation?.Name,
            IsFirstLogin = user.IsFirstLogin,
            ProfileImage = user.ProfileImage,
            AuditLogs = user.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .Select(a => new AuditLogDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    UserName = user.Name,
                    UserEmail = user.Email,
                    Action = a.Action,
                    EntityType = a.EntityType,
                    EntityId = a.EntityId,
                    OldValues = a.OldValues,
                    NewValues = a.NewValues,
                    Timestamp = a.Timestamp,
                    IpAddress = a.IpAddress,
                    UserAgent = a.UserAgent
                })
                .ToList()
        };
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Age = user.Age,
            Address = user.Address,
            RoleId = user.RoleId,
            RoleName = user.Role?.Name,
            DeletedFlag = user.DeletedFlag,
            DesignationId = user.DesignationId,
            DesignationName = user.Designation?.Name,
            IsFirstLogin = user.IsFirstLogin,
            ProfileImage = user.ProfileImage
        };
    }
}