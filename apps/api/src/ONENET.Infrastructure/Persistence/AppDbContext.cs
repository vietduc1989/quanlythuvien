using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ONENET.Application.Common.Interfaces;
using ONENET.Domain.Entities;

namespace ONENET.Infrastructure.Persistence;

/// <summary>
/// DbContext cho ứng dụng ONENET, quản lý các Entity và tương tác với cơ sở dữ liệu.
/// Implement IApplicationDbContext và IUnitOfWork.
/// </summary>
public class AppDbContext : DbContext, IApplicationDbContext, IUnitOfWork
{
    private readonly ICurrentUser _currentUser;
    private readonly IDateTime _dateTime;

    public DbSet<PhieuMuon> PhieuMuons => Set<PhieuMuon>();
    public DbSet<ChiTietPhieuMuon> ChiTietPhieuMuons => Set<ChiTietPhieuMuon>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Reader> Readers => Set<Reader>();

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser, IDateTime dateTime)
        : base(options)
    {
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUser.UserId ?? "System";
        var currentUtc = _dateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = currentUserId;
                    entry.Entity.CreatedAt = currentUtc;
                    entry.Entity.UpdatedBy = currentUserId;
                    entry.Entity.UpdatedAt = currentUtc;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedBy = currentUserId;
                    entry.Entity.UpdatedAt = currentUtc;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}