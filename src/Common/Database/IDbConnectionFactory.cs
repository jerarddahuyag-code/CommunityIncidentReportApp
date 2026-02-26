using Npgsql;
using System.Data;

namespace Common.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync (CancellationToken cancellationToken = default);
}

public class NpgsqlDbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
