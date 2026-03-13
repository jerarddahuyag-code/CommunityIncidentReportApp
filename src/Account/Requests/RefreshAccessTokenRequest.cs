using Common.Mediator;

namespace Account.Requests;
public record RefreshAccessTokenRequest : IRequest<RefreshAccessTokenResponse?>
{
    public required string? RefreshToken { get; init; }
}

public record RefreshAccessTokenResponse
{
    public required string AccessToken { get; init; }

    public required string RefreshToken { get; init; }
}
