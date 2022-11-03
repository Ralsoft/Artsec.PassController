using Artsec.PassController.Dal.Models;
using FirebirdSql.Data.FirebirdClient;

namespace Artsec.PassController.Dal.Repositories;

public class DeviceRepository
{
    private readonly IConnectionProvider _connectionProvider;

    public DeviceRepository(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }
    public async Task<DeviceModel?> GetByIdAsync(int id)
    {
        DeviceModel? result = null;
        var sql =
            "select d.ID_DEV, d.ID_CTRL " +
            "FROM DEVICE d " +
            "WHERE d.ID_DEV = @Id";

        using var connection = _connectionProvider.CreateConnection();
        using var command = new FbCommand(sql, connection);
        command.Parameters.Add("@Id", id);
        connection.Open();

        using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();
        if (reader.HasRows)
        {
            result = new()
            {
                Id = (int)reader["ID_DEV"],
                ControllerId = (int)reader["ID_CTRL"],
            };
        }
        return result;
    }
}
