using Common.Mediator;

namespace Account.Requests;

public record LoginRequest : IRequest<LoginResponse>
{
    public required string Username { get; init; }

    public required string Password { get; init; }
}

public record LoginResponse
{
    public required string AccessToken { get; init; }

    public required string RefreshToken { get; init; }
}
