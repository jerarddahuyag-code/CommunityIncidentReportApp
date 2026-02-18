using Account.Domain;
using Account.Requests;
using Account.Services;
using Common.Helpers;
using Common.Mediator;

namespace Account.RequestHandlers;

public class GenerateInviteRequestHandler(IAccountService accountService) : IRequestHandler<GenerateInviteRequest, string>
{
    public async Task<string> Handle(GenerateInviteRequest request, CancellationToken cancellationToken)
    {
        var code = CodeGenerator.GetRandomString();
        await accountService.CreateInvitation(new InviteDomain
        {
            Code = code,
            IsUsed = false,
            CreatedBy = request.CreatedBy
        }, cancellationToken);
        
        return code;
    }
}
