namespace Incidents.Domain;

public record CommentDomain
{
    public required Guid Id { get; init; }

    public required Guid IncidentId { get; init; }

    public required Guid UserId { get; init; }

    public string? DisplayName { get; init; }

    public required string Content { get; init; }

    public required DateTime CreatedAt { get; init; }
}
