using RoleManagementBackend.Application.DTOs;

namespace RoleManagementBackend.Application.Interfaces;

public interface IVisitorService
{
    Task<VisitorDto> CreateVisitorAsync(VisitorCreateDto request);

    Task<List<VisitorDto>> GetVisitorsAsync();

    Task<VisitorDto?> GetVisitorByIdAsync(long id);
}
