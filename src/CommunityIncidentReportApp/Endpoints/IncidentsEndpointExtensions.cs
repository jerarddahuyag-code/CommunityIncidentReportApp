using Incidents.Requests;
using Common.Mediator;
using Incidents.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunityIncidentReportApp.Endpoints;

public static class IncidentsEndpointExtensions
{
    public static void MapIncidentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("incidents").RequireAuthorization();

        group.MapPost("/", async (IMediator mediator, 
            IFormFile? file,
            [FromForm(Name = "title")] string title,
            [FromForm(Name = "description")] string description,
            [FromForm(Name = "category")] IncidentCategory category,
            [FromForm(Name = "latitude")] double? latitude,
            [FromForm(Name = "longitude")] double? longitude,
            ClaimsPrincipal user, 
            CancellationToken cancellationToken) =>
        {
            var id = await mediator.Send(new CreateIncidentRequest { UserId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!), Title = title, Description = description, Category = category, Latitude = latitude, Longitude = longitude, MediaFile = file }, cancellationToken);
            return TypedResults.Created($"incidents/{id}", id);
        }).DisableAntiforgery();

        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetIncidentsRequest(), cancellationToken);
            return TypedResults.Ok(response);
        });

        group.MapPost("{incidentId}/comments", async (IMediator mediator, Guid incidentId, CreateCommentBody body, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var commentId = await mediator.Send(new CreateCommentRequest { IncidentId = incidentId, UserId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!), Content = body.Content}, cancellationToken);
            return TypedResults.Created($"{incidentId}/comments/{commentId}", commentId);
        });

        group.MapGet("{incidentId}/comments", async (IMediator mediator, Guid incidentId, CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetCommentsRequest { IncidentId = incidentId }, cancellationToken);
            return TypedResults.Ok(response);
        });
    }

    private sealed record CreateCommentBody
    {
        public required string Content { get; init; }
    }
}
