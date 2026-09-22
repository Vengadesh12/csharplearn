using RoleManagementBackend.Domain.Entities;

namespace RoleManagementBackend.Application.DTOs;

public class UserAuthDetails
{
    public User User { get; set; } = null!;

    public string? RoleName { get; set; }

    public string? DesignationName { get; set; }

    public string? DepartmentName { get; set; }

    public List<string> Permissions { get; set; } = new();

    public List<object> Menus { get; set; } = new();
}
