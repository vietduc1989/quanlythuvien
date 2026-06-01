// QUAN-20260601-105011
namespace ONENET.Application.Common.Exceptions;

/// <summary>
/// Exception tùy chỉnh cho trường hợp không tìm thấy tài nguyên.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException()
        : base() { }

    public NotFoundException(string message)
        : base(message) { }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException) { }

    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.") { }
}