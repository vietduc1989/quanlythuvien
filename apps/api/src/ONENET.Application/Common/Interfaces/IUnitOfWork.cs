using System.Threading;
using System.Threading.Tasks;

namespace ONENET.Application.Common.Interfaces;

/// <summary>
/// Interface đại diện cho Unit of Work, quản lý việc lưu các thay đổi.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

