// QUAN-20260601-105011
namespace ONENET.Application.Common.Interfaces;

/// <summary>
/// Interface cung cấp thời gian hiện tại để dễ dàng kiểm thử.
/// </summary>
public interface IDateTime
{
    DateTime UtcNow { get; }
    DateOnly Today { get; }
}