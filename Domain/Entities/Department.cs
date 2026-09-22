namespace RoleManagementBackend.Domain.Entities;

public class Department
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Designation> Designations { get; set; } = new List<Designation>();
}
