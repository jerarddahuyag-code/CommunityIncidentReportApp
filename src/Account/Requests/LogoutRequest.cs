using Common.Mediator;

namespace Account.Requests;
public record LogoutRequest : IRequest<bool>
{
    public required string? RefreshToken { get; init; }
}
