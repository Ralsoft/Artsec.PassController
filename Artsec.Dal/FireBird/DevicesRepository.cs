using Artsec.Carddex.DAL.FireBird.Dtos;
using Artsec.DAL;
using Dapper;
using FirebirdSql.Data.FirebirdClient;

namespace Artsec.Dal.FireBird;

public class DevicesRepository : FireBirdCommandsExecutorBase
{
    private static SemaphoreSlim _semaphore = new SemaphoreSlim(1);

    public DevicesRepository(IConnectionStringProvider connectionStringProvider) : base(connectionStringProvider)
    {
    }
    public async Task<List<DeviceRow>> GetDevicesAsync()
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();
        return (await connection.QueryAsync<DeviceRow>("SELECT * FROM device d WHERE d.ID_READER is NULL")).ToList();
    }
    public async Task UpdateConnectionSettingsAsync(int ctrlId, string connectionSettings)
    {
        await _semaphore.WaitAsync();
        try
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = "UPDATE DEVICE SET CONFIG = @connectionSettings WHERE ID_CTRL = @ctrlId AND ID_READER IS NULL";
            command.Parameters.Add("@connectionSettings", connectionSettings);
            command.Parameters.Add("@ctrlId", ctrlId);
            await command.ExecuteNonQueryAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task UpdateNameAsync(int deviceId, string name)
    {
        if (name == null)
            return;
        if (name?.Length > 50)
            throw new InvalidOperationException("Name length more than 50");

        await _semaphore.WaitAsync();
        try
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = "UPDATE DEVICE SET NAME = @name WHERE ID_CTRL = @deviceId AND ID_READER IS NULL";
            command.Parameters.Add("@name", name);
            command.Parameters.Add("@deviceId", deviceId);
            await command.ExecuteNonQueryAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
