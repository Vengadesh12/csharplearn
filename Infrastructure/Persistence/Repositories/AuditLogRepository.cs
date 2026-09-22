using Microsoft.EntityFrameworkCore;
using RoleManagementBackend.Application.Interfaces;
using RoleManagementBackend.Domain.Entities;
using RoleManagementBackend.Infrastructure.Persistence;

namespace RoleManagementBackend.Infrastructure.Persistence.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly ApplicationDbContext _context;

    public AuditLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AuditLog>> GetAllAsync()
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .Include(a => a.User)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<AuditLog?> GetByIdAsync(int id)
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<AuditLog>> GetByUserIdAsync(int userId)
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .Include(a => a.User)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task AddAsync(AuditLog log)
    {
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }
}
