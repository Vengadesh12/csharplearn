using RoleManagementBackend.Domain.Entities;

namespace RoleManagementBackend.Application.Interfaces;

public interface IVisitorRepository
{
    Task<Visitor> AddAsync(Visitor visitor);

    Task<List<Visitor>> GetAllAsync();

    Task<Visitor?> GetByIdAsync(long id);
}
