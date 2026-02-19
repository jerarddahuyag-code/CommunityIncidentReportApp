using Common.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Common;

public static class ServiceScope
{
    public static IServiceCollection ConfigureMediator(this IServiceCollection services)
    {
        services.AddSingleton<IMediator, SimpleMediator>();
        return services;
    }
}
