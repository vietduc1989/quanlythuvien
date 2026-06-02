// QUAN-20260601-1634
using System;

namespace ONENET.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException()
        : base()
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public NotFoundException(string name, object key)
        : base($"Thực thể \"{name}\" ({key}) không tồn tại trong hệ thống.")
    {
    }
}

