// QUAN-20260601-105011
using ONENET.Application.Common.Interfaces;
using ONENET.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ONENET.Infrastructure.Services;

/// <summary>
/// Triển khai dịch vụ tạo mã độc giả duy nhất theo định dạng DG[YYYYMMDD][SequentialNumber].
/// </summary>
public class ReaderCodeGenerator : IReaderCodeGenerator
{
    private readonly IReaderRepository _readerRepository;
    private readonly IDateTime _dateTime;
    private readonly ILogger<ReaderCodeGenerator> _logger;

    public ReaderCodeGenerator(IReaderRepository readerRepository, IDateTime dateTime, ILogger<ReaderCodeGenerator> logger)
    {
        _readerRepository = readerRepository;
        _dateTime = dateTime;
        _logger = logger;
    }

    public async Task<string> GenerateUniqueCodeAsync(CancellationToken ct = default)
    {
        var datePrefix = _dateTime.UtcNow.ToString("yyyyMMdd");
        var sequentialNumber = 1;
        string newCode;

        while (true)
        {
            // Định dạng số thứ tự 3 chữ số (ví dụ: 001, 002)
            newCode = $"DG{datePrefix}{sequentialNumber:D3}";
            var isUnique = !await _readerRepository.IsReaderCodeUniqueAsync(newCode, ct);

            if (isUnique)
            {
                _logger.LogInformation("Generated unique reader code: {ReaderCode}", newCode);
                return newCode;
            }

            _logger.LogDebug("Reader code '{ReaderCode}' already exists, trying next sequential number.", newCode);
            sequentialNumber++;
            if (sequentialNumber > 999)
            {
                // Nếu vượt quá 999 trong ngày, có thể thêm logic xử lý hoặc báo lỗi
                _logger.LogError("Failed to generate unique reader code for date {DatePrefix} after 999 attempts.", datePrefix);
                throw new InvalidOperationException($"Could not generate a unique reader code for today ({datePrefix}).");
            }
        }
    }
}