using Account.Domain;
using Account.Entities;
using Common.Database;
using Dapper;
using System.ComponentModel;

namespace Account.Services;

public interface IAccountService
{
    Task<Guid> RegisterUser(UserDomain user, CancellationToken cancellationToken);

    Task<UserDomain?> GetUserByUsername(string username, CancellationToken cancellationToken);

    Task CreateInvitation(InviteDomain invite, CancellationToken cancellationToken);

    Task<InviteDomain?> GetInviteByCode(string? code, CancellationToken cancellationToken);

    Task UseInvitationCode(string? code, CancellationToken cancellationToken);

    Task<UserDomain?> GetUserById(Guid userId, CancellationToken cancellationToken);
}
public class AccountService(IDbConnectionFactory connectionFactory) : IAccountService
{
    public async Task<Guid> RegisterUser(UserDomain user, CancellationToken cancellationToken)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new
        {
            user.Id,
            user.Username,
            user.DisplayName,
            user.PasswordHash,
            Role = user.Role.ToString(),
            user.CreatedAt
        };

        await conn.ExecuteAsync(
            """
                insert into users (Id, Username, DisplayName, PasswordHash, Role, CreatedAt)
                values (@Id, @Username, @DisplayName, @PasswordHash, @Role, @CreatedAt)
            """,
            parameters);
        return user.Id;
    }

    public async Task<UserDomain?> GetUserByUsername(string username, CancellationToken cancellationToken)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(cancellationToken);
        return await conn.QuerySingleOrDefaultAsync<UserDomain>(
            """
                select * from users
                where Username = @Username
                limit 1
            """,
            new {Username = username});
    }

    public async Task<UserDomain?> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(cancellationToken);
        return await conn.QuerySingleOrDefaultAsync<UserDomain>(
            """
                select * from users
                where Id = @Id
                limit 1
            """,
            new {Id = userId});
    }

    public async Task CreateInvitation(InviteDomain invite, CancellationToken cancellationToken)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(cancellationToken);
        await conn.ExecuteAsync(
            """
                insert into invites (Code, IsUsed, CreatedBy)
                values (@Code, @IsUsed, @CreatedBy)
            """,
            invite);
    }

    public async Task<InviteDomain?> GetInviteByCode(string? code, CancellationToken cancellationToken)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(cancellationToken);
        return await conn.QuerySingleOrDefaultAsync<InviteDomain>(
            """
                select * from invites
                where Code = @Code
                limit 1
            """,
            new {Code = code});
    }

    public async Task UseInvitationCode(string? code, CancellationToken cancellationToken)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(cancellationToken);
        await conn.ExecuteAsync(
            """
                update invites
                set IsUsed = true
                where Code = @Code
            """,
            new {Code = code});
    }
}
