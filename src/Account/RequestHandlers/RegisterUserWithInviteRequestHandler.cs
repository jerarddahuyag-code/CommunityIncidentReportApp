using Account.Domain;
using Account.Entities;
using Account.Requests;
using Account.Services;
using Common.Mediator;

namespace Account.RequestHandlers;

public class RegisterUserWithInviteRequestHandler(IAccountService accountService) : IRequestHandler<RegisterUserWithInviteRequest, Guid>
{

    public async Task<Guid> Handle(RegisterUserWithInviteRequest request, CancellationToken cancellationToken)
    {
        var invite = await accountService.GetInviteByCode(request.InviteCode, cancellationToken) ?? throw new InvalidOperationException("Invalid invite code.");
        if (invite.IsUsed)
        {
            throw new InvalidOperationException("Invite code is already used up.");
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var id = await accountService.RegisterUser(new UserDomain
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            PasswordHash = hashedPassword,
            DisplayName = request.DisplayName,
            Role = Role.Resident,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await accountService.UseInvitationCode(request.InviteCode, cancellationToken);

        return id;
    }
}
