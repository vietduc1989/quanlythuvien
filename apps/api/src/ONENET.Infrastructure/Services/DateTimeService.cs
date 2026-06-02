// QUAN-20260601-105011
using ONENET.Application.Common.Interfaces;

namespace ONENET.Infrastructure.Services;

/// <summary>
/// Triển khai IDateTime để cung cấp thời gian hiện tại (UTC).
/// </summary>
public class DateTimeService : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}