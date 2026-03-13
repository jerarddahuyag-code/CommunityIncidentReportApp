using Common.Mediator;
using Incidents.Requests;
using Incidents.Services;

namespace Incidents.RequestHandlers;

public class GetCommentsRequestHandler(ICommentService commentService) : IRequestHandler<GetCommentsRequest, GetCommentsResponse>
{
    public async Task<GetCommentsResponse> Handle(GetCommentsRequest request, CancellationToken cancellationToken)
    {
        var comments = await commentService.GetCommentsByIncidentId(request.IncidentId, cancellationToken);

        return new GetCommentsResponse
        {
            Items = [.. comments.Select(x => new GetCommentsResponseItem
            {
                Id = x.Id,
                IncidentId = x.IncidentId,
                UserId = x.UserId,
                Username = x.DisplayName ?? "Unknown",
                Content = x.Content,
                CreatedAt = x.CreatedAt,
            })]
        };
    }
}
