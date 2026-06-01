using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ONENET.Application.Common.Models;
using ONENET.Application.Readers.Commands;
using ONENET.Application.Readers.DTOs;
using ONENET.Application.Readers.Queries;
using ONENET.Domain.Enums;

namespace ONENET.WebAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReadersController : ControllerBase
{
    private readonly ISender _mediator;

    public ReadersController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateReaderCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { success = false, message = result.Error });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { success = true, data = new { readerId = result.Value }, message = "Đăng ký độc giả thành công" });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReaderCommand command)
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

        return Ok(new { success = true, data = new { readerId = id }, message = "Cập nhật hồ sơ độc giả thành công" });
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<ReaderDto>>> Get(
        [FromQuery] string? searchTerm, 
        [FromQuery] string? status, 
        [FromQuery] int pageIndex = 1, 
        [FromQuery] int pageSize = 10)
    {
        ReaderStatus? readerStatus = null;
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ReaderStatus>(status, true, out var parsedStatus))
        {
            readerStatus = parsedStatus;
        }

        var query = new GetReadersQuery
        {
            Search = searchTerm,
            Status = readerStatus,
            PageNumber = pageIndex,
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
        var result = await _mediator.Send(new GetReaderByIdQuery(id));

        if (result.IsFailure)
        {
            return NotFound(new { success = false, message = result.Error });
        }

        return Ok(new { success = true, data = result.Value });
    }
}
