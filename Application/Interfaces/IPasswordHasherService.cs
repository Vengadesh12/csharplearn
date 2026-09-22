using RoleManagementBackend.Domain.Entities;

namespace RoleManagementBackend.Application.Interfaces;

public interface IPasswordHasherService
{
    string HashPassword(User user, string password);

    bool VerifyPassword(User user, string hashedPassword, string providedPassword);
}
