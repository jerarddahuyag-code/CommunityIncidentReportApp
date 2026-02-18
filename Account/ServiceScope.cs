using Account.RequestHandlers;
using Account.Requests;
using Account.Services;
using Common.Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Account;

public static class ServiceScope
{
    public static IServiceCollection ConfigureAccountServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOption = configuration.GetRequiredSection("JWT");
        services.Configure<JwtOption>(jwtOption);
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IRequestHandler<RegisterUserWithoutInviteRequest, Guid>, RegisterUserWithoutInviteRequestHandler>();
        services.AddTransient<IRequestHandler<RegisterUserWithInviteRequest, Guid>, RegisterUserWithInviteRequestHandler>();
        services.AddTransient<IRequestHandler<LoginRequest, string>, LoginRequestHandler>();
        services.AddTransient<IRequestHandler<GenerateInviteRequest, string>, GenerateInviteRequestHandler>();
        return services;
    }
}
