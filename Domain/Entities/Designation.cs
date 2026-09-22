namespace RoleManagementBackend.Domain.Entities;

public class Designation
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? DepartmentId { get; set; }

    public Department? Department { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}
