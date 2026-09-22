using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoleManagementBackend.Application.Common;
using RoleManagementBackend.Application.DTOs;
using RoleManagementBackend.Application.Interfaces;

namespace RoleManagementBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [HttpPost("/api/check/login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        Console.WriteLine("\n================================================================================");
        Console.WriteLine(">>> [STEP 1: CONTROLLER - RECEIVING DATA]");
        Console.WriteLine("    [FILE]     : Api/Controllers/AuthController.cs");
        Console.WriteLine("    [METHOD]   : Login([FromBody] LoginRequestDto request)");
        Console.WriteLine($"   [ENDPOINT] : {Request.Path}");
        Console.WriteLine($"   [PAYLOAD]  : Email = '{request?.Email}', Password = '***'");
        Console.WriteLine("    [ACTION]   : Passing request data to IAuthService.LoginAsync() in Application/Services/AuthService.cs");
        Console.WriteLine("================================================================================");

        if (request == null)
        {
            var failResult = ApiResponse<LoginResponseDto>.Fail("Email and password are required.");
            Console.WriteLine("    [RESPONSE] : return BadRequest(failResult) -> Sent HTTP 400 Bad Request to Postman / Client");
            Console.WriteLine("================================================================================\n");
            return BadRequest(failResult);
        }

        var result = await _authService.LoginAsync(request);

        Console.WriteLine("\n================================================================================");
        Console.WriteLine(">>> [STEP 5: CONTROLLER - SENDING RESPONSE BACK TO CLIENT]");
        Console.WriteLine("    [FILE]     : Api/Controllers/AuthController.cs");
        Console.WriteLine("    [METHOD]   : Login()");
        Console.WriteLine($"   [RESULT]   : Success = {result.Success}, Message = '{result.Message}'");

        if (!result.Success)
        {
            if (result.Message == "Email and password are required.")
            {
                Console.WriteLine("    [RESPONSE] : return BadRequest(result) -> Sent HTTP 400 Bad Request to Postman / Client");
                Console.WriteLine("================================================================================\n");
                return BadRequest(result);
            }

            Console.WriteLine("    [RESPONSE] : return Unauthorized(result) -> Sent HTTP 401 Unauthorized to Postman / Client");
            Console.WriteLine("================================================================================\n");
            return Unauthorized(result);
        }

        Console.WriteLine("    [RESPONSE] : return Ok(result) -> Sent HTTP 200 OK with LoginResponseDto JSON to Postman / Client");
        Console.WriteLine($"   [DATA SENT]: User ID = {result.Data?.Id}, Name = '{result.Data?.Name}', Role = '{result.Data?.RoleName}'");
        Console.WriteLine("================================================================================\n");
        return Ok(result);
    }
}

