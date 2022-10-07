using FirebirdSql.Data.FirebirdClient;

namespace Artsec.PassController.Dal;

public class ConnectionProvider : IConnectionProvider
{
    private readonly string _connectionString;

    public ConnectionProvider(string connectionString)
    {
        _connectionString = connectionString;
    }
    public FbConnection CreateConnection()
    {
        return new FbConnection(_connectionString);
    }
}
