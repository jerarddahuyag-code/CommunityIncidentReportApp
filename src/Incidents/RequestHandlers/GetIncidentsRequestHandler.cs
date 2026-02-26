using Common.Mediator;
using Incidents.Requests;
using Incidents.Services;

namespace Incidents.RequestHandlers;

public class GetIncidentsRequestHandler(IIncidentService incidentService) : IRequestHandler<GetIncidentsRequest, GetIncidentsResponse>
{
    public async Task<GetIncidentsResponse> Handle(GetIncidentsRequest request, CancellationToken cancellationToken)
    {
        var incidents = await incidentService.GetIncidentsAsync(cancellationToken);

        return new GetIncidentsResponse
        {
            Items = [.. incidents.Select(x => new GetIncidentsResponseItem
            {
                Id = x.Id,
                UserId = x.UserId,
                Username = x.DisplayName ?? "Unknown",
                Title = x.Title,
                Description = x.Description,
                Category = x.Category,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Status = x.Status,
                ImageUrl = x.ImageUrl,
                CreatedAt = x.CreatedAt,
            })]
        };
    }
}
