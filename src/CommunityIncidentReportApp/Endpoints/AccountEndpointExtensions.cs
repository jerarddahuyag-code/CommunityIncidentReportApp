using Account;
using Account.Requests;
using Common.Mediator;
using System.Security.Claims;

namespace CommunityIncidentReportApp.Endpoints;

public static class AccountEndpointExtensions
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("accounts");

        group.MapPost("/register", async (IMediator mediator, RegisterUserWithoutInviteRequest request, CancellationToken cancellationToken) =>
        {
            var id = await mediator.Send(request, cancellationToken);
            return TypedResults.Created($"/register/{id}", id);
        });//.RequireAuthorization("Inviter");

        group.MapPost("/register/{inviteCode}", async (IMediator mediator, RegisterUserWithInviteRequest request, string inviteCode, CancellationToken cancellationToken) =>
        {
            var id = await mediator.Send(request with { InviteCode = inviteCode}, cancellationToken);
            return TypedResults.Created($"/register/{id}", id);
        });

        group.MapPost("/login", async (IMediator mediator, LoginRequest request, HttpResponse httpResponse, CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(request, cancellationToken);
            httpResponse.Cookies.Append(DefaultCookieOptions.RefreshTokenKey, response.RefreshToken, DefaultCookieOptions.GetDefaultCookieOpitons());
            return TypedResults.Ok(response.AccessToken);
        });

        group.MapPost("/logout", async (IMediator mediator, HttpRequest httpRequest, HttpResponse httpResponse, CancellationToken cancellationToken) =>
        {
            var refreshToken = httpRequest.Cookies[DefaultCookieOptions.RefreshTokenKey];
            await mediator.Send(new LogoutRequest { RefreshToken = refreshToken }, cancellationToken);
            httpResponse.Cookies.Delete(DefaultCookieOptions.RefreshTokenKey, DefaultCookieOptions.GetDefaultCookieOpitons());
            return TypedResults.Ok();
        }).RequireAuthorization();

        group.MapPost("/generate-invite", async (IMediator mediator, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var token = await mediator.Send(new GenerateInviteRequest { CreatedBy = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!) }, cancellationToken);
            return TypedResults.Created($"/generate-invite/{token}", token);
        }).RequireAuthorization("Inviter");

        group.MapPost("/refresh", async (HttpContext context, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var refreshToken = context.Request.Cookies[DefaultCookieOptions.RefreshTokenKey];
            var response = await mediator.Send(new RefreshAccessTokenRequest { RefreshToken = refreshToken }, cancellationToken); 
            if (response == null || string.IsNullOrEmpty(response.AccessToken))
            {
                return (IResult)TypedResults.Unauthorized();
            }

            context.Response.Cookies.Append(DefaultCookieOptions.RefreshTokenKey, response.RefreshToken, DefaultCookieOptions.GetDefaultCookieOpitons());

            return TypedResults.Ok(response.AccessToken);
        });
    }
}
