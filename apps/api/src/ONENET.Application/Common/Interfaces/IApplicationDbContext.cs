using Microsoft.EntityFrameworkCore;
using ONENET.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ONENET.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Book> Books { get; }
    DbSet<Reader> Readers { get; }
    DbSet<PhieuMuon> PhieuMuons { get; }
    DbSet<ChiTietPhieuMuon> ChiTietPhieuMuons { get; }
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
