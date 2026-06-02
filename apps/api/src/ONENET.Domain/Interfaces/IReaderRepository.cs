// QUAN-20260601-105011
using ONENET.Domain.Entities;
using ONENET.Domain.Enums;

namespace ONENET.Domain.Interfaces;

/// <summary>
/// Interface định nghĩa các thao tác CRUD cho entity Reader.
/// </summary>
public interface IReaderRepository
{
    /// <summary>
    /// Lấy độc giả theo ID.
    /// </summary>
    Task<Reader?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Lấy độc giả theo mã độc giả.
    /// </summary>
    Task<Reader?> GetByReaderCodeAsync(string readerCode, CancellationToken ct = default);

    /// <summary>
    /// Kiểm tra số điện thoại có tồn tại cho độc giả khác hay không.
    /// </summary>
    Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber, Guid? excludeReaderId = null, CancellationToken ct = default);

    /// <summary>
    /// Kiểm tra mã độc giả có tồn tại hay không.
    /// </summary>
    Task<bool> IsReaderCodeUniqueAsync(string readerCode, CancellationToken ct = default);

    /// <summary>
    /// Thêm độc giả mới.
    /// </summary>
    Task AddAsync(Reader reader, CancellationToken ct = default);

    /// <summary>
    /// Cập nhật thông tin độc giả.
    /// </summary>
    void Update(Reader reader);

    /// <summary>
    /// Lấy danh sách độc giả có phân trang, tìm kiếm và lọc.
    /// </summary>
    Task<IReadOnlyList<Reader>> GetPagedListAsync(
        string? searchTerm,
        ReaderStatus? status,
        int pageIndex,
        int pageSize,
        string sortBy,
        string sortOrder,
        CancellationToken ct = default);

    /// <summary>
    /// Đếm tổng số độc giả dựa trên điều kiện tìm kiếm và lọc.
    /// </summary>
    Task<int> CountAsync(string? searchTerm, ReaderStatus? status, CancellationToken ct = default);
}