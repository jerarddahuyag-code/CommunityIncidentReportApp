using Account.Requests;
using Account.Services;
using Common.Mediator;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace Account.RequestHandlers;

public class LoginRequestHandler(IAccountService accountService, IAuthService authService) : IRequestHandler<LoginRequest, string>
{
    public async Task<string> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await accountService.GetUserByUsername(request.Username, cancellationToken) ?? throw new Exception("Invalid Credentials");
        var isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isValid) throw new Exception("Invalid Credentials");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.GivenName, user.DisplayName),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        return authService.GetToken(claims);
    }
}
