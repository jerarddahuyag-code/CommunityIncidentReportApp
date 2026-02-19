namespace Account.Domain;

public class InviteDomain
{
    public required string Code { get; init; }

    public required bool IsUsed { get; init; }

    public required Guid CreatedBy { get; init; }
}
