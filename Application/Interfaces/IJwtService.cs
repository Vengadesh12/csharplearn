namespace RoleManagementBackend.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(int userId, string email, string roleName, IEnumerable<string> permissions);
}
