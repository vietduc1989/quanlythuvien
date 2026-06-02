using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ONENET.Application.Loans.Commands;
using ONENET.Application.Loans.Queries;
using ONENET.Domain.Common;

namespace ONENET.WebAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ISender _sender;

    public LoansController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLoan([FromBody] CreateLoanRequest request)
    {
        var command = new CreateLoanCommand(request.ReaderId, request.BookIds);
        var result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Failed(
                "Yêu cầu không hợp lệ hoặc vi phạm quy tắc nghiệp vụ.",
                new List<ApiError> { new ApiError { Field = "businessRule", Message = result.Error } }
            ));
        }

        return StatusCode(201, ApiResponse<CreateLoanResponse>.Succeeded(
            new CreateLoanResponse(result.Value),
            "Phiếu mượn được tạo thành công."
        ));
    }

    [HttpPut("{loanDetailId:guid}/return")]
    public async Task<IActionResult> RecordBookReturn(Guid loanDetailId)
    {
        var command = new RecordBookReturnCommand(loanDetailId);
        var result = await _sender.Send(command);

        if (result.IsFailure)
        {
            if (result.Error.Contains("không tồn tại"))
            {
                return NotFound(ApiResponse<object>.Failed(result.Error));
            }
            return BadRequest(ApiResponse<object>.Failed(result.Error));
        }

        return Ok(ApiResponse<RecordBookReturnResult>.Succeeded(
            result.Value,
            "Sách đã được ghi nhận trả thành công."
        ));
    }

    [HttpGet]
    public async Task<IActionResult> GetLoans(
        [FromQuery] Guid? readerId,
        [FromQuery] bool? isOverdue,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetLoansQuery(readerId, isOverdue, pageNumber, pageSize);
        var result = await _sender.Send(query);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Failed(result.Error));
        }

        return Ok(ApiResponse<object>.Succeeded(result.Value, "Lấy danh sách phiếu mượn thành công."));
    }
}

// Request and Response wrapper models
public record CreateLoanRequest(Guid ReaderId, List<Guid> BookIds);
public record CreateLoanResponse(Guid LoanId);

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<ApiError>? Errors { get; set; }

    public static ApiResponse<T> Succeeded(T data, string? message = null) => new() { Success = true, Data = data, Message = message };
    public static ApiResponse<T> Failed(string message, List<ApiError>? errors = null) => new() { Success = false, Message = message, Errors = errors };
}

public class ApiError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
