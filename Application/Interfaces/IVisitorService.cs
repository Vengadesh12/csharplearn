using RoleManagementBackend.Application.DTOs;
using RoleManagementBackend.Domain.Entities;

namespace RoleManagementBackend.Application.Interfaces;

public interface IVisitorService
{
    Task<Visitor> CreateVisitorAsync(VisitorCreateDto request);
    Task<List<Visitor>> GetVisitorsAsync();
    Task<Visitor?> GetVisitorByIdAsync(long id);
}
