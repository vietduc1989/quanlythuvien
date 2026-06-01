// QUAN-20260601-105011
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ONENET.Application.Common.Behaviors;
using ONENET.Application.Common.Interfaces;

namespace ONENET.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Đăng ký các service chung khác trong Application layer nếu có
        services.AddScoped<IUnitOfWork, UnitOfWorkPlaceholder>(); // TODO: Implement IUnitOfWork in Infrastructure
        
        return services;
    }

    // Tạm thời để tránh lỗi build khi chưa implement IUnitOfWork
    private class UnitOfWorkPlaceholder : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }
    }
}