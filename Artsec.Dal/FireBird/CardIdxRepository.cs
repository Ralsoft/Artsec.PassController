using Artsec.DAL;

namespace Artsec.Dal.FireBird;

public class CardIdxRepository : FireBirdCommandsExecutorBase
{
    private static SemaphoreSlim _semaphore = new SemaphoreSlim(1);
    public CardIdxRepository(IConnectionStringProvider connectionStringProvider) : base(connectionStringProvider)
    {
    }

    public async Task UpdateAsync(int deviceId, string card, string result)
    {
        await _semaphore.WaitAsync();
        try
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText =
                $"UPDATE CARDIDX " +
                $"SET " +
                    $"ID_DB = 1," +
                    $"LOAD_TIME = @time," +
                    $"LOAD_RESULT = @result " +
                $"WHERE (ID_CARD = @card) AND (ID_DEV = @deviceId)";
            command.Parameters.Add("@time", DateTime.Now);
            command.Parameters.Add("@result", result);
            command.Parameters.Add("@card", card);
            command.Parameters.Add("@deviceId", deviceId);
            await command.ExecuteNonQueryAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
