using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Services;
using Microsoft.Extensions.Configuration;

namespace Artsec.PassController.Dal;

public class DefaultConnectionProvider : IConnectionProvider
{
    private readonly IConfiguration _configuration;
    private const string ConnectionStringName = "DefaultConnection";

    public DefaultConnectionProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public FbConnection CreateConnection()
    {
        return new FbConnection(_configuration.GetConnectionString(ConnectionStringName));
    }
}
