using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ONENET.Domain.Entities;

namespace ONENET.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Book> Books { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
