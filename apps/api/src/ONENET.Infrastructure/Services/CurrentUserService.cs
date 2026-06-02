// QUAN-20260601-105011
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ONENET.Application.Common.Interfaces;

namespace ONENET.Infrastructure.Services;

/// <summary>
/// Dịch vụ để lấy thông tin người dùng hiện tại từ HttpContext.
/// </summary>
public class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name); // Or UPN/email
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string roleName)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(roleName) ?? false;
    }
}