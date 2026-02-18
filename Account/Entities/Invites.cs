namespace Account.Entities;

public record Invites
{
    public required int Id { get; init; }

    public required string Code { get; init; }

    public required bool IsUsed { get; init; }

    public required Guid CreatedBy { get; init; }
}
