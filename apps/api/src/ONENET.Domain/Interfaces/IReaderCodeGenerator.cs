// QUAN-20260601-105011
namespace ONENET.Domain.Interfaces;

/// <summary>
/// Interface định nghĩa dịch vụ tạo mã độc giả duy nhất.
/// </summary>
public interface IReaderCodeGenerator
{
    /// <summary>
    /// Tạo một mã độc giả duy nhất theo định dạng DG[YYYYMMDD][SequentialNumber].
    /// </summary>
    Task<string> GenerateUniqueCodeAsync(CancellationToken ct = default);
}