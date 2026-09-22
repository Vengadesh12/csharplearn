using RoleManagementBackend.Application.Common;
using RoleManagementBackend.Application.DTOs;
using RoleManagementBackend.Application.Interfaces;

namespace RoleManagementBackend.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasherService passwordHasher,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        Console.WriteLine("\n--------------------------------------------------------------------------------");
        Console.WriteLine(">>> [STEP 2: SERVICE - PROCESSING DATA]");
        Console.WriteLine("    [FILE]     : Application/Services/AuthService.cs");
        Console.WriteLine("    [METHOD]   : LoginAsync(LoginRequestDto request)");
        Console.WriteLine("    [LINE 25]  : Validating input fields (Email & Password)...");

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            Console.WriteLine("    [LINE 27]  : Validation failed -> Email or password is empty!");
            return ApiResponse<LoginResponseDto>.Fail("Email and password are required.");
        }

        Console.WriteLine($"    [LINE 30]  : Input is valid. Calling IUserRepository.GetAuthDetailsByEmailAsync('{request.Email.Trim()}')...");
        var authDetails = await _userRepository.GetAuthDetailsByEmailAsync(request.Email.Trim());

        Console.WriteLine("\n--------------------------------------------------------------------------------");
        Console.WriteLine(">>> [STEP 2 CONTINUED: SERVICE - RESUMING IN AuthService.cs]");
        Console.WriteLine("    [FILE]     : Application/Services/AuthService.cs");
        Console.WriteLine("    [LINE 31]  : Checking if user details were returned from repository...");

        if (authDetails == null)
        {
            Console.WriteLine("    [LINE 33]  : User NOT found in database -> Returning fail response.");
            return ApiResponse<LoginResponseDto>.Fail("Invalid email or password.");
        }

        var user = authDetails.User;
        Console.WriteLine($"    [LINE 36]  : User found in DB -> User ID: {user.Id}, Name: '{user.Name}', DeletedFlag: {user.DeletedFlag}");

        Console.WriteLine("    [LINE 38]  : Checking account status (user.DeletedFlag != 1)...");
        if (user.DeletedFlag != 1)
        {
            Console.WriteLine("    [LINE 40]  : Account is INACTIVE (DeletedFlag != 1) -> Returning fail response.");
            return ApiResponse<LoginResponseDto>.Fail("User account is inactive. Please contact administrator.");
        }

        Console.WriteLine("    [LINE 43]  : Account is active. Calling IPasswordHasherService.VerifyPassword()...");
        bool isPasswordValid = _passwordHasher.VerifyPassword(user, user.Password, request.Password);
        Console.WriteLine($"    [LINE 44]  : Password check result -> isPasswordValid = {isPasswordValid}");

        if (!isPasswordValid)
        {
            Console.WriteLine("    [LINE 46]  : Password mismatch -> Returning fail response.");
            return ApiResponse<LoginResponseDto>.Fail("Invalid email or password.");
        }

        Console.WriteLine("    [LINE 49]  : Password is correct! Calling IJwtService.GenerateToken()...");
        string token = _jwtService.GenerateToken(
            user.Id,
            user.Email,
            authDetails.RoleName ?? "",
            authDetails.Permissions
        );
        Console.WriteLine($"    [LINE 54]  : JWT Token generated successfully (Length: {token.Length} chars).");

        Console.WriteLine("    [LINE 56]  : Assembling LoginResponseDto with User details, Roles, Permissions, and Menus...");
        var responseDto = new LoginResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            ProfileImage = user.ProfileImage,
            RoleId = user.RoleId,
            RoleName = authDetails.RoleName,
            DepartmentName = authDetails.DepartmentName,
            DesignationName = authDetails.DesignationName,
            Permissions = authDetails.Permissions,
            Menus = authDetails.Menus,
            Token = token,
            IsFirstLogin = user.IsFirstLogin
        };

        Console.WriteLine("    [LINE 72]  : Returning ApiResponse<LoginResponseDto>.Ok() back to AuthController.cs");
        Console.WriteLine("--------------------------------------------------------------------------------");

        return ApiResponse<LoginResponseDto>.Ok(responseDto, "Login successful.");
    }
}
