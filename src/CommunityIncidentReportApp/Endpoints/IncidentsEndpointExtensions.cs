using Common.Mediator;
using Incidents.Domain;
using Incidents.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunityIncidentReportApp.Endpoints;

public static class IncidentsEndpointExtensions
{
    public static void MapIncidentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("incidents").RequireAuthorization();

        group.MapPost("", async (IMediator mediator, 
            IFormFile file,
            [FromForm(Name = "title")] string title,
            [FromForm(Name = "description")] string description,
            [FromForm(Name = "category")] IncidentCategory category,
            [FromForm(Name = "latitude")] double? latitude,
            [FromForm(Name = "longitude")] double? longitude,
            ClaimsPrincipal user, 
            CancellationToken cancellationToken) =>
        {
            var id = await mediator.Send(new CreateIncidentRequest { UserId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!), Title = title, Description = description, Category = category, Latitude = latitude, Longitude = longitude, MediaFile = file }, cancellationToken);
            return TypedResults.Created("incidents", id);
        }).DisableAntiforgery();
    }
}
