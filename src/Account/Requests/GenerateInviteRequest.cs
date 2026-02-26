using Common.Mediator;

namespace Account.Requests;

public record GenerateInviteRequest : IRequest<string>
{
    public required Guid CreatedBy { get; init; }
}
