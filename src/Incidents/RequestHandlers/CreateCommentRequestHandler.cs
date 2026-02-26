using Incidents.Domain;
using Common.Mediator;
using Incidents.Requests;
using Incidents.Services;

namespace Incidents.RequestHandlers;

public class CreateCommentRequestHandler(ICommentService commentService) : IRequestHandler<CreateCommentRequest, Guid>
{
    public async Task<Guid> Handle(CreateCommentRequest request, CancellationToken cancellationToken)
    {
        var id = await commentService.CreateCommentAsync(new CommentDomain
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            IncidentId = request.IncidentId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        return id;
    }
}
