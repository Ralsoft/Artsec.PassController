using Artsec.DAL;
using FirebirdSql.Data.FirebirdClient;

namespace Artsec.Dal.FireBird;

public abstract class FireBirdCommandsExecutorBase
{
    protected readonly IConnectionStringProvider _connectionStringProvider;
    public FireBirdCommandsExecutorBase(IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    protected FbConnection CreateConnection()
    {
        try
        {
            return new FbConnection(_connectionStringProvider.ConnectionString);
        }
        catch
        {
            return null;
        }
    }
}