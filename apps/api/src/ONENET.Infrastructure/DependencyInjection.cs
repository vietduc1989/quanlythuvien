using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ONENET.Application.Common.Interfaces;
using ONENET.Infrastructure.Persistence;
using ONENET.Infrastructure.Persistence.Repositories;
using ONENET.Infrastructure.Services;

namespace ONENET.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddTransient<ICurrentUser, CurrentUserService>();

        // Add other repositories and infrastructure services here

        return services;
    }
}