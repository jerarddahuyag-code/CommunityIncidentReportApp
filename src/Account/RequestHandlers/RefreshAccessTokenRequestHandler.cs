using Account.Domain;
using Account.Requests;
using Account.Services;
using Common.Mediator;

namespace Account.RequestHandlers;
public class RefreshAccessTokenRequestHandler(IAccountService accountService, IAuthService authService) : IRequestHandler<RefreshAccessTokenRequest, RefreshAccessTokenResponse?>
{
    public async Task<RefreshAccessTokenResponse?> Handle(RefreshAccessTokenRequest request, CancellationToken cancellationToken)
    {
        if (request.RefreshToken is null)
        {
            return null;
        }

        var validatedRefreshToken = await authService.GetRefreshToken(request.RefreshToken, cancellationToken);

        if (validatedRefreshToken is null) {
            return null;
        }

        var user = await accountService.GetUserById(validatedRefreshToken.UserId, cancellationToken) ?? throw new Exception("Invalid User Id");

        var newAccessToken = authService.GenerateClaimsFromUser(user);
        var newRefreshToken = authService.GenerateRefreshToken();

        await authService.RevokeRefreshToken(validatedRefreshToken.Token);

        await authService.SaveRefreshToken(new RefreshTokenDomain()
        {
            Token = newRefreshToken,
            UserId = user.Id,
            CreatedOn = DateTime.UtcNow,
            ExpiresOn = DateTime.UtcNow.AddDays(7),
            RevokedOn = null
        });

        return new()
        {
            AccessToken = authService.GetToken(newAccessToken),
            RefreshToken = newRefreshToken,
        };
    }
}
