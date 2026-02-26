namespace Account;

public record JwtOption
{
    public required string ValidAudience { get; init; }

    public required string ValidIssuer { get; init; }

    public required string SecretKey { get; init; }
}
