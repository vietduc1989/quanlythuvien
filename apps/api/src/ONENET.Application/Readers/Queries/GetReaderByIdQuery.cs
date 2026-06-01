using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ONENET.Application.Common.Interfaces;
using ONENET.Application.Readers.DTOs;
using ONENET.Domain.Common;

namespace ONENET.Application.Readers.Queries;

public record GetReaderByIdQuery(Guid Id) : IRequest<Result<ReaderDto>>;

public class GetReaderByIdQueryHandler : IRequestHandler<GetReaderByIdQuery, Result<ReaderDto>>
{
    private readonly IAppDbContext _context;

    public GetReaderByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ReaderDto>> Handle(GetReaderByIdQuery request, CancellationToken cancellationToken)
    {
        var reader = await _context.Readers
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

        if (reader == null)
        {
            return Result.Failure<ReaderDto>($"Không tìm thấy chi tiết độc giả với ID '{request.Id}'.");
        }

        var dto = new ReaderDto
        {
            Id = reader.Id,
            ReaderCode = reader.ReaderCode,
            FullName = reader.FullName,
            DateOfBirth = reader.DateOfBirth,
            PhoneNumber = reader.PhoneNumber,
            Email = reader.Email,
            Address = reader.Address,
            RegistrationDate = reader.RegistrationDate,
            ExpiryDate = reader.ExpiryDate,
            Status = reader.Status.ToString()
        };

        return Result.Success(dto);
    }
}
