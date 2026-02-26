using Common.Mediator;

namespace Incidents.Requests;

public record GetCommentsRequest : IRequest<GetCommentsResponse>
{
    public required Guid IncidentId { get; init; }
}

public record GetCommentsResponse
{
    public required GetCommentsResponseItem[] Items { get; init; }
}

public record GetCommentsResponseItem
{
    public required Guid Id { get; init; }

    public required Guid IncidentId { get; init; }

    public required Guid UserId { get; init; }

    public required string UserName { get; init; }

    public required string Content { get; init; }

    public required DateTime CreatedAt { get; init; }
}
