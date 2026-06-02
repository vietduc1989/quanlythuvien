// QUAN-20260601-1634
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ONENET.Application.Common.Interfaces;
using ONENET.Domain.Entities;

namespace ONENET.Infrastructure.Persistence;

public class AppDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUser _currentUser;

    public DbSet<PhieuMuon> PhieuMuons => Set<PhieuMuon>();
    public DbSet<ChiTietPhieuMuon> ChiTietPhieuMuons => Set<ChiTietPhieuMuon>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Reader> Readers => Set<Reader>();

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser)
        : base(options)
    {
        _currentUser = currentUser;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            var userId = _currentUser.UserId ?? "System";
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.IsDeleted = false;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = userId;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = userId;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

