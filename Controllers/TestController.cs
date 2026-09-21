using Microsoft.AspNetCore.Mvc;

namespace RoleManagementBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Test controller is showing data ");
        }
    }
}