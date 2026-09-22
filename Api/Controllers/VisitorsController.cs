using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoleManagementBackend.Application.Common;
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
            return BadRequest(ApiResponse<VisitorDto>.Fail("Request body cannot be null."));
        }

        if (!ModelState.IsValid)
        {
            var errors = string.Join("; ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            return BadRequest(ApiResponse<VisitorDto>.Fail("Validation failed.", error: errors));
        }

        var visitor = await _visitorService.CreateVisitorAsync(request);
        return Ok(ApiResponse<VisitorDto>.Ok(visitor, "Visitor registered successfully."));
    }

    [HttpGet]
    public async Task<IActionResult> GetVisitors()
    {
        var visitors = await _visitorService.GetVisitorsAsync();
        return Ok(ApiResponse<List<VisitorDto>>.Ok(visitors));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetVisitor(long id)
    {
        var visitor = await _visitorService.GetVisitorByIdAsync(id);

        if (visitor == null)
        {
            return NotFound(ApiResponse<VisitorDto>.Fail($"Visitor with ID {id} was not found."));
        }

        return Ok(ApiResponse<VisitorDto>.Ok(visitor));
    }
}