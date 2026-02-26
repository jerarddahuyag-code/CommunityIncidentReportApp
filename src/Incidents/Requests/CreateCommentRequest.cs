using Common.Mediator;

namespace Incidents.Requests;

public record CreateCommentRequest : IRequest<Guid>
{
    public required Guid IncidentId { get; init; }

    public required Guid UserId { get; init; }

    public required string Content { get; init; }
}
