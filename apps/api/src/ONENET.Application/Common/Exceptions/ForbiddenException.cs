// QUAN-20260601-1634
using System;

namespace ONENET.Application.Common.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException()
            : base("You are not authorized to perform this action.")
        {
        }

        public ForbiddenException(string message)
            : base(message)
        {
        }

        public ForbiddenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}