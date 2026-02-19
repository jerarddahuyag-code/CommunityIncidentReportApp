using Common.Database;
using Dapper;
using Incidents.Domain;

namespace Incidents.Services;

public interface IIncidentService
{
    Task<Guid> CreateIncidentAsync(IncidentDomain incident, CancellationToken cancellationToken);
}
public class IncidentService(IDbConnectionFactory connectionFactory) : IIncidentService
{
    public async Task<Guid> CreateIncidentAsync(IncidentDomain incident, CancellationToken cancellationToken)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(cancellationToken);
        await conn.ExecuteAsync(
            """
                insert into incidents(Id, UserId, Title, Description, Category, Latitude, Longitude, Status, ImageUrl)
                values (@Id, @UserId, @Title, @Description, @Category, @Latitude, @Longitude, @Status, @ImageUrl)
            """
            , incident);

        return incident.Id;
    }
}
