using Account.Entities;
using Common.Mediator;

namespace Account.Requests;

public record RegisterUserWithInviteRequest : IRequest<Guid>
{
    public required string Username { get; init; }

    public required string Password { get; init; }

    public required string DisplayName { get; init; }

    public string? InviteCode { get; init; }
}
