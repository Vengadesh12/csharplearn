namespace RoleManagementBackend.Application.DTOs;

public class LoginResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? ProfileImage { get; set; }

    public int? RoleId { get; set; }

    public string? RoleName { get; set; }

    public string? DepartmentName { get; set; }

    public string? DesignationName { get; set; }

    public List<string> Permissions { get; set; } = new();

    public List<object> Menus { get; set; } = new();

    public string Token { get; set; } = string.Empty;

    public bool IsFirstLogin { get; set; }
}
