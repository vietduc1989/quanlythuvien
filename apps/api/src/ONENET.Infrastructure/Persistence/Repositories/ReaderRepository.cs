// QUAN-20260601-105011
using Microsoft.EntityFrameworkCore;
using ONENET.Domain.Entities;
using ONENET.Domain.Enums;
using ONENET.Domain.Interfaces;
using ONENET.Infrastructure.Persistence;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace ONENET.Infrastructure.Persistence.Repositories;

/// <summary>
/// Triển khai interface IReaderRepository sử dụng Entity Framework Core.
/// </summary>
public class ReaderRepository : IReaderRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<ReaderRepository> _logger;

    public ReaderRepository(AppDbContext context, ILogger<ReaderRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Reader?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving reader by ID: {ReaderId}", id);
        return await _context.Readers.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task<Reader?> GetByReaderCodeAsync(string readerCode, CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving reader by ReaderCode: {ReaderCode}", readerCode);
        return await _context.Readers.AsNoTracking().FirstOrDefaultAsync(r => r.ReaderCode == readerCode, ct);
    }

    public async Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber, Guid? excludeReaderId = null, CancellationToken ct = default)
    {
        _logger.LogDebug("Checking phone number uniqueness for {PhoneNumber}, excluding reader ID: {ExcludeReaderId}", phoneNumber, excludeReaderId);
        if (excludeReaderId.HasValue)
        {
            return await _context.Readers.AnyAsync(r => r.PhoneNumber == phoneNumber && r.Id != excludeReaderId.Value, ct);
        }
        return await _context.Readers.AnyAsync(r => r.PhoneNumber == phoneNumber, ct);
    }

    public async Task<bool> IsReaderCodeUniqueAsync(string readerCode, CancellationToken ct = default)
    {
        _logger.LogDebug("Checking reader code uniqueness for {ReaderCode}", readerCode);
        return await _context.Readers.AnyAsync(r => r.ReaderCode == readerCode, ct);
    }

    public async Task AddAsync(Reader reader, CancellationToken ct = default)
    {
        _logger.LogInformation("Adding new reader with ID: {ReaderId}, ReaderCode: {ReaderCode}", reader.Id, reader.ReaderCode);
        await _context.Readers.AddAsync(reader, ct);
    }

    public void Update(Reader reader)
    {
        _logger.LogInformation("Updating reader with ID: {ReaderId}, ReaderCode: {ReaderCode}", reader.Id, reader.ReaderCode);
        _context.Readers.Update(reader); // EF Core sẽ tự động đánh dấu là Modified
    }

    public async Task<IReadOnlyList<Reader>> GetPagedListAsync(
        string? searchTerm,
        ReaderStatus? status,
        int pageIndex,
        int pageSize,
        string sortBy,
        string sortOrder,
        CancellationToken ct = default)
    {
        _logger.LogDebug("Getting paged list of readers. SearchTerm: '{SearchTerm}', Status: {Status}, PageIndex: {PageIndex}, PageSize: {PageSize}, SortBy: {SortBy}, SortOrder: {SortOrder}",
            searchTerm, status, pageIndex, pageSize, sortBy, sortOrder);

        IQueryable<Reader> query = _context.Readers.AsNoTracking();

        // Filtering
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(r =>
                r.FullName.ToLower().Contains(lowerSearchTerm) ||
                r.PhoneNumber.ToLower().Contains(lowerSearchTerm) ||
                (r.Email != null && r.Email.ToLower().Contains(lowerSearchTerm)) ||
                r.ReaderCode.ToLower().Contains(lowerSearchTerm));
        }

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        // Sorting
        Expression<Func<Reader, object>> keySelector = sortBy.ToLower() switch
        {
            "readercode" => r => r.ReaderCode,
            "registrationdate" => r => r.RegistrationDate,
            "expirydate" => r => r.ExpiryDate,
            _ => r => r.FullName // Default sort by FullName
        };

        query = sortOrder.ToLower() == "desc"
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);

        // Pagination
        var readers = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return readers;
    }

    public async Task<int> CountAsync(string? searchTerm, ReaderStatus? status, CancellationToken ct = default)
    {
        IQueryable<Reader> query = _context.Readers.AsNoTracking();

        // Filtering (similar to GetPagedListAsync)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(r =>
                r.FullName.ToLower().Contains(lowerSearchTerm) ||
                r.PhoneNumber.ToLower().Contains(lowerSearchTerm) ||
                (r.Email != null && r.Email.ToLower().Contains(lowerSearchTerm)) ||
                r.ReaderCode.ToLower().Contains(lowerSearchTerm));
        }

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        return await query.CountAsync(ct);
    }
}