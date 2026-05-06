using Common.Mediator;
using Incidents.Requests;
using Incidents.Services;

namespace Incidents.RequestHandlers;

public class UpdateIncidentStatusRequestHandler(IIncidentService incidentService) : IRequestHandler<UpdateIncidentStatusRequest, Guid>
{
    public Task<Guid> Handle(UpdateIncidentStatusRequest request, CancellationToken cancellationToken)
    {
        var id = incidentService.UpdateIncidentStatus(request.Id, request.NewStatus, cancellationToken);
        return id;
    }
}
