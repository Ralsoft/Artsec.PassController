using Artsec.Carddex.DAL.FireBird.Dtos;
using Artsec.DAL;
using Dapper;
using FirebirdSql.Data.FirebirdClient;
using System.Data;

namespace Artsec.Dal.FireBird;

public class ArtonitProcedures : FireBirdCommandsExecutorBase
{
    public enum FireBirdEventCode
    {
        None = 0,
        UnknownCard = 46,
        ValidCard = 50,
        TimeZoneError = 47,
        DoorOpened = 53,
        DoorClosed = 54,
        DoorOpenedManual = 49,
    }
    public ArtonitProcedures(IConnectionStringProvider connectionStringProvider) : base(connectionStringProvider)
    {
    }
    public async Task<int> InsertDeviceEventAsync(FireBirdEventCode eventCode, int controllerId, string cardNumber, DateTime time)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var cmd = new FbCommand("DEVICEEVENTS_INSERT", connection);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@ID_DB", 1);
        cmd.Parameters.Add("@ID_EVENTTYPE", (int)eventCode);
        cmd.Parameters.Add("@ID_CTRL", controllerId);

        var param = new FbParameter("@ID_READER", FbDbType.Integer)
        {
            Value = 0
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
    public async Task<int> RefreshDeviceAsync(int deviceId)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var cmd = new FbCommand("CARDIDX_REFRESH", connection);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@ID_DEV", deviceId);

        var responce = await cmd.ExecuteScalarAsync();
        if (int.TryParse(responce?.ToString(), out int result))
            return result;

        return -1;
    }

    public async Task<IEnumerable<CardInDevGetListRow>> GetCommandsForDevices()
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        var procedure = "CARDINDEV_GETLIST";
        var values = new { ID_DB = 1 };
        var results = await connection.QueryAsync<CardInDevGetListRow>(procedure, values, commandType: CommandType.StoredProcedure);

        return results;
    }
}
