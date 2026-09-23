using System;

namespace RoleManagementBackend.Domain.Entities;

public class VisitorActivity
{
    public long Id { get; set; }

    public long VisitorId { get; set; }

    public string Activity { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Visitor? Visitor { get; set; }
}