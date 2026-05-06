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
        services.AddTransient<IRequestHandler<GetIncidentsRequest, GetIncidentsResponse>, GetIncidentsRequestHandler>();
        services.AddTransient<IFileService, FileService>();
        services.AddTransient<IIncidentService, IncidentService>();

        services.AddTransient<ICommentService, CommentService>();
        services.AddTransient<IRequestHandler<GetCommentsRequest, GetCommentsResponse>, GetCommentsRequestHandler>();
        services.AddTransient<IRequestHandler<CreateCommentRequest, Guid>, CreateCommentRequestHandler>();
        services.AddTransient<IRequestHandler<UpdateIncidentStatusRequest, Guid>, UpdateIncidentStatusRequestHandler>();
        return services;
    }
}
