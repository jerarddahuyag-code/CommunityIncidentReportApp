using Common.Mediator;

namespace Account.Requests;

public record LoginRequest : IRequest<string>
{
    public required string Username { get; init; }

    public required string Password { get; init; }
}
