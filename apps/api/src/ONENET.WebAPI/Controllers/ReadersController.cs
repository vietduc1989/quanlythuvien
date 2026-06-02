// QUAN-20260601-105011
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONENET.Application.Common.Models;
using ONENET.Application.Features.Readers.Commands.CreateReader;
using ONENET.Application.Features.Readers.Commands.UpdateReader;
using ONENET.Application.Features.Readers.DTOs;
using ONENET.Application.Features.Readers.Queries.GetReaderById;
using ONENET.Application.Features.Readers.Queries.GetReadersList;
using ONENET.WebAPI.Filters;

namespace ONENET.WebAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReadersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReadersController> _logger;

    public ReadersController(IMediator mediator, ILogger<ReadersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Tạo mới một hồ sơ độc giả trong hệ thống. (QUAN-20260601-105011-SRS01)
    /// </summary>
    /// <param name="command">Dữ liệu độc giả cần tạo mới.</param>
    /// <returns>Thông tin độc giả đã được tạo.</returns>
    /// <response code="201">Tạo độc giả thành công.</response>
    /// <response code="400">Dữ liệu đầu vào không hợp lệ hoặc số điện thoại đã tồn tại.</response>
    /// <response code="401">Chưa xác thực.</response>
    /// <response code="403">Không có quyền truy cập (yêu cầu role 'Librarian').</response>
    [HttpPost]
    [AuthorizeRoles("Librarian")]
    [ProducesResponseType(typeof(ApiResponse<CreateReaderResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateReaderCommand command)
    {
        _logger.LogInformation("Received request to create a new reader with FullName: {FullName}", command.FullName);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Reader created successfully with ID: {ReaderId}", result.Value!.ReaderId);
            return CreatedAtAction(nameof(GetById), new { id = result.Value!.ReaderId }, ApiResponse<CreateReaderResponse>.Succeeded(result.Value));
        }

        _logger.LogWarning("Failed to create reader: {ErrorMessage}", result.Error);
        return BadRequest(ApiResponse.Failed(result.Error!));
    }

    /// <summary>
    /// Lấy danh sách độc giả với khả năng tìm kiếm, lọc và phân trang. (QUAN-20260601-105011-SRS02)
    /// </summary>
    /// <param name="query">Các tiêu chí tìm kiếm, lọc, phân trang và sắp xếp.</param>
    /// <returns>Danh sách độc giả đã phân trang.</returns>
    /// <response code="200">Trả về danh sách độc giả.</response>
    /// <response code="400">Các tham số truy vấn không hợp lệ.</response>
    /// <response code="401">Chưa xác thực.</response>
    /// <response code="403">Không có quyền truy cập (yêu cầu role 'Librarian' hoặc 'Viewer').</response>
    [HttpGet]
    [AuthorizeRoles("Librarian", "Viewer")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ReaderListItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetList([FromQuery] GetReadersListQuery query)
    {
        _logger.LogInformation("Received request to get reader list. SearchTerm: {SearchTerm}, Status: {Status}, Page: {PageIndex}, Size: {PageSize}",
            query.SearchTerm, query.Status, query.PageIndex, query.PageSize);

        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<PaginatedList<ReaderListItemDto>>.Succeeded(result.Value!));
        }

        _logger.LogWarning("Failed to retrieve reader list: {ErrorMessage}", result.Error);
        return BadRequest(ApiResponse.Failed(result.Error!));
    }

    /// <summary>
    /// Lấy thông tin chi tiết của một độc giả theo ID. (QUAN-20260601-105011-SRS02)
    /// </summary>
    /// <param name="id">ID của độc giả.</param>
    /// <returns>Thông tin chi tiết của độc giả.</returns>
    /// <response code="200">Trả về thông tin chi tiết độc giả.</response>
    /// <response code="401">Chưa xác thực.</response>
    /// <response code="403">Không có quyền truy cập (yêu cầu role 'Librarian' hoặc 'Viewer').</response>
    /// <response code="404">Không tìm thấy độc giả.</response>
    [HttpGet("{id}")]
    [AuthorizeRoles("Librarian", "Viewer")]
    [ProducesResponseType(typeof(ApiResponse<ReaderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        _logger.LogInformation("Received request to get reader by ID: {ReaderId}", id);
        var result = await _mediator.Send(new GetReaderByIdQuery(id));

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<ReaderDto>.Succeeded(result.Value!));
        }

        _logger.LogWarning("Failed to retrieve reader with ID {ReaderId}: {ErrorMessage}", id, result.Error);
        // NotFoundException will be handled by GlobalExceptionMiddleware to return 404
        return NotFound(ApiResponse.Failed(result.Error!));
    }

    /// <summary>
    /// Cập nhật thông tin của một độc giả đã tồn tại. (QUAN-20260601-105011-SRS03)
    /// </summary>
    /// <param name="id">ID của độc giả cần cập nhật.</param>
    /// <param name="command">Dữ liệu cập nhật.</param>
    /// <returns>ID của độc giả đã cập nhật.</returns>
    /// <response code="200">Cập nhật độc giả thành công.</response>
    /// <response code="400">Dữ liệu đầu vào không hợp lệ hoặc số điện thoại đã tồn tại.</response>
    /// <response code="401">Chưa xác thực.</response>
    /// <response code="403">Không có quyền truy cập (yêu cầu role 'Librarian').</response>
    /// <response code="404">Không tìm thấy độc giả.</response>
    /// <response code="409">Xung đột dữ liệu (optimistic concurrency).</response>
    [HttpPut("{id}")]
    [AuthorizeRoles("Librarian")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReaderCommand command)
    {
        _logger.LogInformation("Received request to update reader with ID: {ReaderId}", id);

        if (id != command.ReaderId)
        {
            _logger.LogWarning("Mismatched Reader ID in path ({PathId}) and body ({BodyId}) for update request.", id, command.ReaderId);
            return BadRequest(ApiResponse.Failed("Reader ID in path must match Reader ID in request body."));
        }

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Reader with ID: {ReaderId} updated successfully.", result.Value);
            return Ok(ApiResponse<Guid>.Succeeded(result.Value));
        }

        // Lỗi xung đột sẽ được GlobalExceptionMiddleware bắt và trả về 409 Conflict
        // Các lỗi khác (validation, not found) cũng sẽ được middleware xử lý
        _logger.LogWarning("Failed to update reader with ID {ReaderId}: {ErrorMessage}", id, result.Error);
        return BadRequest(ApiResponse.Failed(result.Error!));
    }
}