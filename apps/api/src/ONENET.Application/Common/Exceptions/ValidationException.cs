// QUAN-20260601-105011
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ONENET.Application.Common.Exceptions;

/// <summary>
/// Exception tùy chỉnh cho các lỗi validation nghiệp vụ.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation errors occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }
}

/// <summary>
/// Đại diện cho một lỗi validation cụ thể.
/// </summary>
public class ValidationError
{
    public string Field { get; set; } = default!;
    public string Message { get; set; } = default!;
}