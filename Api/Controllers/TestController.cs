using Microsoft.AspNetCore.Mvc;

namespace RoleManagementBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "RoleManagementBackend API is running successfully." });
    }
}