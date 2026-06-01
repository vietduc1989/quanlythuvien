// QUAN-20260601-105011
namespace ONENET.Application.Common.Interfaces;

/// <summary>
/// Cung cấp thông tin về người dùng hiện tại đang thực hiện yêu cầu.
/// </summary>
public interface ICurrentUser
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string roleName);
}