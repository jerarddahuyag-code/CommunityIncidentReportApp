using Common.Mediator;
using Incidents.RequestHandlers;
using Incidents.Requests;
using Incidents.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Incidents;
public static class ServiceScope
{
    public static IServiceCollection ConfigureIncidentServices(this IServiceCollection services)
    {
        services.AddTransient<IRequestHandler<CreateIncidentRequest, Guid>, CreateIncidentRequestHandler>();
        services.AddTransient<IFileService, FileService>();
        services.AddTransient<IIncidentService, IncidentService>();

        return services;
    }
}
