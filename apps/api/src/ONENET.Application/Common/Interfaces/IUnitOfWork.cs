// QUAN-20260601-1634
using System.Threading;
using System.Threading.Tasks;

namespace ONENET.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}