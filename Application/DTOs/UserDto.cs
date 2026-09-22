namespace RoleManagementBackend.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public int? Age { get; set; }

    public string? Address { get; set; }

    public int? RoleId { get; set; }

    public string? RoleName { get; set; }

    public int DeletedFlag { get; set; }

    public int? DesignationId { get; set; }

    public string? DesignationName { get; set; }

    public bool IsFirstLogin { get; set; }

    public string? ProfileImage { get; set; }
}

public class UserWithAuditLogsDto : UserDto
{
    public List<AuditLogDto> AuditLogs { get; set; } = new();
}