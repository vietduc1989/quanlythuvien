using MediatR;
using Microsoft.AspNetCore.Mvc;
using ONENET.Application.Books.Commands;
using ONENET.Application.Books.Queries;
using ONENET.Domain.Enums;
using ONENET.WebAPI.Models;
using System;
using System.Threading.Tasks;

namespace ONENET.WebAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, ApiResponse.SuccessResult(new { id = result }, "Thêm sách thành công"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse.FailureResult("ID sách trong URL không trùng khớp với request body."));
        }

        await _mediator.Send(command);
        return Ok(ApiResponse.SuccessResult(null, "Cập nhật sách thành công"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteBookCommand(id));
        return Ok(ApiResponse.SuccessResult(null, "Xóa sách thành công"));
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] BookStatus? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetBooksQuery(search, category, status, pageNumber, pageSize));
        return Ok(ApiResponse.SuccessResult(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetBookByIdQuery(id));
        return Ok(ApiResponse.SuccessResult(result));
    }
}
