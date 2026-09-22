using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoleManagementBackend.Application.DTOs;
using RoleManagementBackend.Application.Interfaces;

namespace RoleManagementBackend.Api.Controllers;

[ApiController]
[Route("api/visitors")]
[AllowAnonymous]
public class VisitorsController : ControllerBase
{
    private readonly IVisitorService _visitorService;

    public VisitorsController(IVisitorService visitorService)
    {
        _visitorService = visitorService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateVisitor([FromBody] VisitorCreateDto? request)
    {
        if (request == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Request body cannot be null"
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "Validation failed",
                errors = ModelState
            });
        }

        try
        {
            var visitor = await _visitorService.CreateVisitorAsync(request);

            return Ok(new
            {
                success = true,
                message = "Visitor registered successfully",
                data = new
                {
                    visitor.Id,
                    visitor.Name,
                    visitor.Email,
                    visitor.Phone,
                    visitor.Company,
                    visitor.Purpose,
                    visitor.VisitDate,
                    visitor.CreatedAt
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Failed to save visitor",
                error = ex.Message
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetVisitors()
    {
        try
        {
            var visitors = await _visitorService.GetVisitorsAsync();

            return Ok(new
            {
                success = true,
                data = visitors
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Failed to get visitors",
                error = ex.Message
            });
        }
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetVisitor(long id)
    {
        try
        {
            var visitor = await _visitorService.GetVisitorByIdAsync(id);

            if (visitor == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Visitor not found"
                });
            }

            return Ok(new
            {
                success = true,
                data = visitor
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Failed to get visitor",
                error = ex.Message
            });
        }
    }
}