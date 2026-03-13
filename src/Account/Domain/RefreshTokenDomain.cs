namespace Account.Domain;
public record RefreshTokenDomain
{
    public required string Token { get; init; }

    public required Guid UserId { get; init; }

    public required DateTime ExpiresOn { get; init; }

    public required DateTime CreatedOn { get; init; }

    public required DateTime? RevokedOn { get; init; }
}
