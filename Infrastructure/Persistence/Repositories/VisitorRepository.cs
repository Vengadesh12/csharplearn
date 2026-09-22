using Microsoft.EntityFrameworkCore;
using RoleManagementBackend.Application.Interfaces;
using RoleManagementBackend.Domain.Entities;
using RoleManagementBackend.Infrastructure.Persistence;

namespace RoleManagementBackend.Infrastructure.Persistence.Repositories;

public class VisitorRepository : IVisitorRepository
{
    private readonly ApplicationDbContext _context;

    public VisitorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Visitor> AddAsync(Visitor visitor)
    {
        await _context.Visitors.AddAsync(visitor);
        await _context.SaveChangesAsync();
        return visitor;
    }

    public async Task<List<Visitor>> GetAllAsync()
    {
        return await _context.Visitors
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<Visitor?> GetByIdAsync(long id)
    {
        return await _context.Visitors
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
