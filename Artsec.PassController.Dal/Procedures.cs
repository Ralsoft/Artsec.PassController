using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Artsec.PassController.Dal;

public class Dto
{
    [Column("EVENT_TYPE")]
    public int EVENT_TYPE { get; set; }
    [Column("ID_PEP")]
    public int ID_PEP { get; set; }
}
public class Procedures
{
    private readonly IConnectionProvider _connectionProvider;

    public Procedures(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<int> ValidatePass(int deviceId, string identifier)
    {
        using var connection = _connectionProvider.CreateConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "VALIDATEPASS";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("ID_DEV", deviceId);
        command.Parameters.Add("ID_CARD", identifier);
        command.Parameters.Add("GRZ", null);

        var result = await command.ExecuteScalarAsync();
        return (int)result;
    }

}