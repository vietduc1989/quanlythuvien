// QUAN-20260601-105011
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ONENET.Application.Common.Interfaces;
using ONENET.Domain.Entities;

namespace ONENET.Infrastructure.Persistence;

/// <summary>
/// DbContext cho ứng dụng ONENET, quản lý các Entity và tương tác với cơ sở dữ liệu.
/// Implement IUnitOfWork để quản lý transaction và SaveChanges.
/// </summary>
public class AppDbContext : DbContext, IAppDbContext, IUnitOfWork
{
    private readonly ICurrentUser _currentUser;
    private readonly IDateTime _dateTime;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser, IDateTime dateTime)
        : base(options)
    {
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public DbSet<Reader> Readers => Set<Reader>();

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
                    entry.Entity.LastModifiedBy = currentUserId;
                    entry.Entity.LastModifiedAt = currentUtc;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = currentUserId;
                    entry.Entity.LastModifiedAt = currentUtc;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Interface cho DbContext để sử dụng trong Application layer,
/// giúp tránh phụ thuộc trực tiếp vào EntityFramework.
/// </summary>
public interface IAppDbContext
{
    DbSet<Reader> Readers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface đại diện cho Unit of Work, quản lý việc lưu các thay đổi.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}