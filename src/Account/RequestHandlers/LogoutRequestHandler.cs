using Account.Requests;
using Account.Services;
using Common.Mediator;
using Microsoft.AspNetCore.Http;

namespace Account.RequestHandlers;
public record LogoutRequestHandler(IAuthService authService) : IRequestHandler<LogoutRequest, bool>
{
    public async Task<bool> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        if (request.RefreshToken is null)
        {
            return true;
        }

        await authService.RevokeRefreshToken(request.RefreshToken);

        return true;
    }
}
