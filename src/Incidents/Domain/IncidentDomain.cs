using Incidents.Requests;
using Microsoft.AspNetCore.Http;

namespace Incidents.Domain;

public class IncidentDomain
{
    public required Guid Id { get; set; }

    public required Guid UserId { get; init; }

    public required string Title { get; init; }

    public required string Description { get; init; }

    public required IncidentCategory Category { get; init; }

    public double? Latitude { get; init; }

    public double? Longitude { get; init; }

    public required IncidentStatus Status { get; init; }

    public string? ImageUrl { get; init; }
}

public enum IncidentCategory
{
    Security = 0,
    Maintenance,
    Wildlife,
    Suggestion,
    Other
}

public enum IncidentStatus
{
    Reported = 0,
    Resolved,
    Denied
}
