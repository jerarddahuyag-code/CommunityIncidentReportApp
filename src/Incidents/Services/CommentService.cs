using Common.Database;
using Dapper;
using Incidents.Domain;

namespace Incidents.Services;

public interface ICommentService
{
    Task<Guid> CreateCommentAsync(CommentDomain comment, CancellationToken cancellationToken);
    Task<CommentDomain[]> GetCommentsByIncidentId(Guid incidentId, CancellationToken cancellationToken);
}

public class CommentService(IDbConnectionFactory connectionFactory) : ICommentService
{
    public async Task<Guid> CreateCommentAsync(CommentDomain comment, CancellationToken cancellationToken)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(cancellationToken);

        await conn.ExecuteAsync(
            """
                insert into comments (Id, IncidentId, UserId, Content, CreatedAt)
                values (@Id, @IncidentId, @UserId, @Content, @CreatedAt)
            """, comment);

        return comment.Id;
    }

    public async Task<CommentDomain[]> GetCommentsByIncidentId(Guid incidentId, CancellationToken cancellationToken)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var comments = await conn.QueryAsync<CommentDomain>(
            """
                select c.Id, c.IncidentId, c.UserId, u.DisplayName, c.Content, c.CreatedAt 
                from comments c
                left join users u on c.UserId = u.Id
                where c.IncidentId = @IncidentId
                order by c.CreatedAt desc
            """, new { IncidentId = incidentId });

        return [.. comments];
    }
}
