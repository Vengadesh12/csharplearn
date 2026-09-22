namespace RoleManagementBackend.Domain.Entities;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public int? Age { get; set; }

    public string? Address { get; set; }

    public int? RoleId { get; set; }

    public Role? Role { get; set; }

    public int DeletedFlag { get; set; } = 1;

    public int? DesignationId { get; set; }

    public Designation? Designation { get; set; }

    public bool IsFirstLogin { get; set; }

    public string? ProfileImage { get; set; }

    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}