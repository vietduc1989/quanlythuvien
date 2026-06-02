// QUAN-20260601-105011
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ONENET.Application.Common.Interfaces;
using ONENET.Domain.Interfaces;
using ONENET.Infrastructure.Persistence;
using ONENET.Infrastructure.Persistence.Repositories;
using ONENET.Infrastructure.Services;
using Serilog;

namespace ONENET.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
                .LogTo(Log.Logger.Debug, LogLevel.Information)); // Logging EF Core queries

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>()); // AppDbContext cũng là IUnitOfWork

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<ICurrentUser, CurrentUserService>(); // TODO: Implement in detail based on Auth design
        services.AddTransient<IReaderCodeGenerator, ReaderCodeGenerator>();

        services.AddScoped<IReaderRepository, ReaderRepository>();

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        return services;
    }
}
