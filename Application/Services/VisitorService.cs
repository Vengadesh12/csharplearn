using RoleManagementBackend.Application.DTOs;
using RoleManagementBackend.Application.Interfaces;
using RoleManagementBackend.Domain.Entities;

namespace RoleManagementBackend.Application.Services;

public class VisitorService : IVisitorService
{
    private readonly IVisitorRepository _visitorRepository;

    public VisitorService(IVisitorRepository visitorRepository)
    {
        _visitorRepository = visitorRepository;
    }

    public async Task<VisitorDto> CreateVisitorAsync(VisitorCreateDto request)
    {
        var visitor = new Visitor
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone.Trim(),
            Company = request.Company.Trim(),
            Purpose = request.Purpose.Trim(),
            VisitDate = DateTime.SpecifyKind(request.VisitDate.Date, DateTimeKind.Unspecified),
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        var savedVisitor = await _visitorRepository.AddAsync(visitor);
        return MapToDto(savedVisitor);
    }

    public async Task<List<VisitorDto>> GetVisitorsAsync()
    {
        var visitors = await _visitorRepository.GetAllAsync();
        return visitors.Select(MapToDto).ToList();
    }

    public async Task<VisitorDto?> GetVisitorByIdAsync(long id)
    {
        var visitor = await _visitorRepository.GetByIdAsync(id);
        return visitor == null ? null : MapToDto(visitor);
    }

    private static VisitorDto MapToDto(Visitor visitor)
    {
        return new VisitorDto
        {
            Id = visitor.Id,
            Name = visitor.Name,
            Email = visitor.Email,
            Phone = visitor.Phone,
            Company = visitor.Company,
            Purpose = visitor.Purpose,
            VisitDate = visitor.VisitDate,
            CreatedAt = visitor.CreatedAt
        };
    }
}