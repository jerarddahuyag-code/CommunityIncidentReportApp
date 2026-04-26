using Account.Domain;
using Account.Requests;
using Account.Services;
using Common.Mediator;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace Account.RequestHandlers;

public class LoginRequestHandler(IAccountService accountService, IAuthService authService) : IRequestHandler<LoginRequest, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await accountService.GetUserByUsername(request.Username, cancellationToken) ?? throw new Exception("Invalid Credentials");
        var isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isValid)
        {
            throw new Exception("Invalid Credentials");
        }

        var claims = authService.GenerateClaimsFromUser(user);

        var refreshToken = authService.GenerateRefreshToken();

        await authService.SaveRefreshToken(new RefreshTokenDomain()
        {
            Token = refreshToken,
            UserId = user.Id,
            CreatedOn = DateTime.UtcNow,
            ExpiresOn = DateTime.UtcNow.AddDays(1),
            RevokedOn = null
        });

        return new()
        {
            AccessToken = authService.GetToken(claims),
            RefreshToken = refreshToken
        };
    }
}
