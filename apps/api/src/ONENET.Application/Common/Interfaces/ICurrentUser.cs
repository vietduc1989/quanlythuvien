// QUAN-20260601-1634
using System;

namespace ONENET.Application.Common.Interfaces
{
    public interface ICurrentUser
    {
        string? UserId { get; }
        string? UserName { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string roleName);
    }
}