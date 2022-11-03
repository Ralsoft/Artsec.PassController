using FirebirdSql.Data.FirebirdClient;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Artsec.PassController.Dal;

public class Procedures
{
    private readonly IConnectionProvider _connectionProvider;

    public Procedures(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<int> ValidatePass(int deviceId, string identifier)
    {
        if (identifier.Length > 12)
            throw new ValidationException($"Identifier max length is 12");
        using var connection = _connectionProvider.CreateConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "VALIDATEPASS";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("ID_DEV", deviceId);
        command.Parameters.Add("ID_CARD", identifier);
        command.Parameters.Add("GRZ", null);
        var result = await command.ExecuteScalarAsync();
        return (int)result;
    }
    public async Task<int> InsertDeviceEventAsync(int eventCode, int controllerId, int readerId, string cardNumber, DateTime time)
    {
        using var connection = _connectionProvider.CreateConnection();
        await connection.OpenAsync();
        using var cmd = new FbCommand("DEVICEEVENTS_INSERT", connection);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@ID_DB", 1);
        cmd.Parameters.Add("@ID_EVENTTYPE", (int)eventCode);
        cmd.Parameters.Add("@ID_CTRL", controllerId);

        var param = new FbParameter("@ID_READER", FbDbType.Integer)
        {
            Value = readerId
        };
        cmd.Parameters.Add(param);

        cmd.Parameters.Add("@NOTE", cardNumber);
        cmd.Parameters.Add("@TIME", time);
        cmd.Parameters.Add("@ID_VIDEO", null);
        cmd.Parameters.Add("@ID_USER", null);
        cmd.Parameters.Add("@ESS1", null);
        cmd.Parameters.Add("@ESS2", null);
        cmd.Parameters.Add("@IDSOURCE", 1);
        cmd.Parameters.Add("@IDSERVERTS", 1);

        var responce = await cmd.ExecuteScalarAsync();
        if (int.TryParse(responce?.ToString(), out int result))
            return result;

        return -1;
    }
}