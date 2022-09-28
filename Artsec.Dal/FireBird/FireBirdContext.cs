using Artsec.DAL;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.Dal.FireBird;

public class FireBirdContext
{
    private readonly IConnectionStringProvider _connectionStringProvider;

    public FireBirdContext(IConnectionStringProvider connectionStringProvider)
    {
        Procedures = new(connectionStringProvider);
        CardInDev = new(connectionStringProvider);
        Devices = new(connectionStringProvider);
        CardIdx = new(connectionStringProvider);
        Card = new(connectionStringProvider);
        _connectionStringProvider = connectionStringProvider;
    }
    public ArtonitProcedures Procedures { get; }
    public CardInDevRepository CardInDev { get; }
    public DevicesRepository Devices { get; }
    public CardIdxRepository CardIdx { get; }
    public CardRepository Card { get; }


    public async Task<bool> CheckConnection()
    {
        if (string.IsNullOrEmpty(_connectionStringProvider.ConnectionString))
            return false;
        try
        {
            await using var connection = new FbConnection(_connectionStringProvider.ConnectionString);
            await connection.OpenAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

}
