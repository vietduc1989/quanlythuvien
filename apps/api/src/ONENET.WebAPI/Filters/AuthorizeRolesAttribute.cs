// QUAN-20260601-105011
using Microsoft.AspNetCore.Authorization;

namespace ONENET.WebAPI.Filters;

/// <summary>
/// Custom Authorization attribute để chỉ định nhiều roles cho một endpoint.
/// </summary>
public class AuthorizeRolesAttribute : AuthorizeAttribute
{
    public AuthorizeRolesAttribute(params string[] roles)
    {
        Roles = string.Join(",", roles);
    }
}