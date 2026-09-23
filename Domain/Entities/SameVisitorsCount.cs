using System;

namespace RoleManagementBackend.Domain.Entities;

public class SameVisitorsCount
{
    public long Id { get; set; }

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public int VisitCount { get; set; }

    public DateTime LastVisitDate { get; set; }
}
