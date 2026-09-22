using Microsoft.AspNetCore.Identity;
using RoleManagementBackend.Application.Interfaces;
using RoleManagementBackend.Domain.Entities;

namespace RoleManagementBackend.Infrastructure.Security;

public class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<User> _hasher = new();

    public string HashPassword(User user, string password)
    {
        return _hasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string hashedPassword, string providedPassword)
    {
        Console.WriteLine("\n................................................................................");
        Console.WriteLine(">>> [STEP 3.1: SECURITY - VERIFYING PASSWORD]");
        Console.WriteLine("    [FILE]     : Infrastructure/Security/PasswordHasherService.cs");
        Console.WriteLine("    [METHOD]   : VerifyPassword(User user, string hashedPassword, string providedPassword)");
        Console.WriteLine($"   [USER]     : Email = '{user.Email}', Id = {user.Id}");
        Console.WriteLine("    [ACTION]   : Comparing hashed password with provided password using ASP.NET Identity PasswordHasher...");

        var result = _hasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        bool isValid = result != PasswordVerificationResult.Failed;

        Console.WriteLine($"   [RESULT]   : Password match status = {(isValid ? "MATCHED (Success)" : "FAILED (Wrong password)")}");
        Console.WriteLine("................................................................................");
        return isValid;
    }
}
