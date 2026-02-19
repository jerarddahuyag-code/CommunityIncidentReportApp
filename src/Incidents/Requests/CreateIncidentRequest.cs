using Common.Mediator;
using Incidents.Domain;
using Microsoft.AspNetCore.Http;

namespace Incidents.Requests;

public record CreateIncidentRequest : IRequest<Guid>
{
    public required Guid UserId { get; init; }

    public required string Title { get; init; }

    public required string Description { get; init; }

    public required IncidentCategory Category { get; init; }

    public string? Latitude { get; init; }

    public string? Longitude { get; init; }

    public IFormFile? MediaFile { get; init; }
}
