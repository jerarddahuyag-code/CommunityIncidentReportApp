using Common.Mediator;
using Incidents.Domain;

namespace Incidents.Requests;

public record UpdateIncidentStatusRequest : IRequest<Guid>
{
    public required Guid Id { get; init; }

    public required IncidentStatus NewStatus { get; init; }
}
