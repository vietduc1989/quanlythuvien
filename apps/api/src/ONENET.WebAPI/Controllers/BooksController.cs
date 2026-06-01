using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ONENET.Application.Books.Commands;
using ONENET.Application.Books.DTOs;
using ONENET.Application.Books.Queries;
using ONENET.Application.Common.Models;

namespace ONENET.WebAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BooksController : ControllerBase
{
    private readonly ISender _mediator;

    public BooksController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateBookCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { success = false, message = result.Error });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { success = true, data = new { id = result.Value }, message = "Thêm sách thành công" });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { success = false, message = "ID không trùng khớp." });
        }

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { success = false, message = result.Error });
        }

        return Ok(new { success = true, message = "Cập nhật sách thành công" });
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteBookCommand(id));

        if (result.IsFailure)
        {
            return BadRequest(new { success = false, message = result.Error });
        }

        return Ok(new { success = true, message = "Xóa sách thành công" });
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<BookDto>>> Get([FromQuery] string? search, [FromQuery] string? category, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetBooksQuery
        {
            Search = search,
            Category = category,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetBookByIdQuery(id));

        if (result.IsFailure)
        {
            return NotFound(new { success = false, message = result.Error });
        }

        return Ok(new { success = true, data = result.Value });
    }
}
