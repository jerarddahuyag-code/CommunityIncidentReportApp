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
            return TypedResults.Created("/register", id);
        });//.RequireAuthorization("Inviter");

        group.MapPost("/register/{inviteCode}", async (IMediator mediator, RegisterUserWithInviteRequest request, string inviteCode, CancellationToken cancellationToken) =>
        {
            var id = await mediator.Send(request, cancellationToken);
            return TypedResults.Created("/register", id);
        });

        group.MapPost("/login", async (IMediator mediator, LoginRequest request, CancellationToken cancellationToken) =>
        {
            var id = await mediator.Send(request, cancellationToken);
            return TypedResults.Ok(id);
        });

        group.MapPost("/generate-invite", async (IMediator mediator, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var id = await mediator.Send(new GenerateInviteRequest { CreatedBy = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!)}, cancellationToken);
            return TypedResults.Created("/generate-invite", id);
        }).RequireAuthorization("Inviter");
    }
}
