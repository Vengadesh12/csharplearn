using Microsoft.EntityFrameworkCore;
using RoleManagementBackend.Application.DTOs;
using RoleManagementBackend.Application.Interfaces;
using RoleManagementBackend.Domain.Entities;
using RoleManagementBackend.Infrastructure.Persistence;

namespace RoleManagementBackend.Application.Services;

public class VisitorService : IVisitorService
{
    private readonly ApplicationDbContext _context;

    public VisitorService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Visitor> CreateVisitorAsync(VisitorCreateDto request)
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

        _context.Visitors.Add(visitor);
        await _context.SaveChangesAsync();

        return visitor;
    }

    public async Task<List<Visitor>> GetVisitorsAsync()
    {
        return await _context.Visitors
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<Visitor?> GetVisitorByIdAsync(long id)
    {
        return await _context.Visitors
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}