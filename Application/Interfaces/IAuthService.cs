using RoleManagementBackend.Application.Common;
using RoleManagementBackend.Application.DTOs;

namespace RoleManagementBackend.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
}
