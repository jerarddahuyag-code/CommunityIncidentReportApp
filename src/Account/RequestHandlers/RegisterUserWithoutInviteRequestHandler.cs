using Account.Domain;
using Account.Requests;
using Account.Services;
using Common.Mediator;

namespace Account.RequestHandlers;

public class RegisterUserWithoutInviteRequestHandler(IAccountService accountService) : IRequestHandler<RegisterUserWithoutInviteRequest, Guid>
{
    public async Task<Guid> Handle(RegisterUserWithoutInviteRequest request, CancellationToken cancellationToken)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        return await accountService.RegisterUser(new UserDomain
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            PasswordHash = hashedPassword,
            DisplayName = request.DisplayName,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);
    }
}
