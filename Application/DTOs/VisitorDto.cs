namespace RoleManagementBackend.Application.DTOs;

public class VisitorDto
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Company { get; set; } = string.Empty;

    public string Purpose { get; set; } = string.Empty;

    public DateTime VisitDate { get; set; }

    public DateTime CreatedAt { get; set; }
}
