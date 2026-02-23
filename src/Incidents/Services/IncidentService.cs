using Common.Database;
using Dapper;
using Incidents.Domain;

namespace Incidents.Services;

public interface IIncidentService
{
    Task<Guid> CreateIncidentAsync(IncidentDomain incident, CancellationToken cancellationToken);

    Task<IncidentDomain[]> GetIncidentsAsync (CancellationToken cancellationToken);

    Task<Guid> UpdateIncidentStatus(Guid id, IncidentStatus status, CancellationToken cancellationToken);
}
public class IncidentService(IDbConnectionFactory connectionFactory) : IIncidentService
{
    public async Task<Guid> CreateIncidentAsync(IncidentDomain incident, CancellationToken cancellationToken)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var parameters = new
        {
            incident.Id,
            incident.UserId,
            incident.Title,
            incident.Description,
            Category = incident.Category.ToString(),
            incident.Latitude,
            incident.Longitude,
            Status = incident.Status.ToString(),   
            incident.ImageUrl,
            incident.CreatedAt
        };

        await conn.ExecuteAsync(
            """
                insert into incidents(Id, UserId, Title, Description, Category, Latitude, Longitude, Status, ImageUrl, CreatedAt)
                values (@Id, @UserId, @Title, @Description, @Category, @Latitude, @Longitude, @Status, @ImageUrl, @CreatedAt)
            """
            , parameters);

        return incident.Id;
    }

    public async Task<IncidentDomain[]> GetIncidentsAsync(CancellationToken cancellationToken)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var incidents = await conn.QueryAsync<IncidentDomain>(
            """
                select i.Id, i.UserId, u.DisplayName, i.Title, i.Description, i.Category, i.Latitude, i.Longitude, i.Status, i.ImageUrl, i.CreatedAt
                from incidents i
                left join users u on i.UserId = u.Id
                order by i.CreatedAt desc
                limit 20
            """
            );

        return [.. incidents];
    }

    public async Task<Guid> UpdateIncidentStatus(Guid id, IncidentStatus status, CancellationToken cancellationToken)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(cancellationToken);

        await conn.ExecuteAsync(
            """
                update incidents
                set Status = @Status
                where Id = @Id
            """, new { Id = id, Status = status.ToString() });

        return id;
    }
}
