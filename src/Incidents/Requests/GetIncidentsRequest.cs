using Common.Mediator;
using Incidents.Domain;

namespace Incidents.Requests;

public record GetIncidentsRequest : IRequest<GetIncidentsResponse>
{
}

public class GetIncidentsResponse
{
    public required GetIncidentsResponseItem[] Items { get; init; }
}

public class GetIncidentsResponseItem
{
    public required Guid Id { get; init; }

    public required Guid UserId { get; init; }

    public required string Username { get; init; }

    public required string Title { get; init; }

    public required string Description { get; init; }

    public required IncidentCategory Category { get; init; }

    public required double? Latitude { get; init; }

    public required double? Longitude { get; init; }

    public required IncidentStatus Status { get; init; }

    public required string? ImageUrl { get; init; }

    public required DateTime CreatedAt { get; init; }
}
