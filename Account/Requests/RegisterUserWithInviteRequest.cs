using Account.Entities;
using Common.Mediator;

namespace Account.Requests;

public class RegisterUserWithInviteRequest : IRequest<Guid>
{
    public required string Username { get; init; }

    public required string Password { get; init; }

    public required string DisplayName { get; init; }

    public required string InviteCode { get; init; }
}
