using Artsec.Carddex.DAL.FireBird.Dtos;
using Artsec.DAL;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Artsec.Dal.FireBird;

public class CardInDevRepository : FireBirdCommandsExecutorBase
{
    public CardInDevRepository(IConnectionStringProvider connectionStringProvider) : base(connectionStringProvider)
    {
    }

    private static SemaphoreSlim _semaphore = new SemaphoreSlim(1);
    public async Task DeleteAsync(int id)
    {
        await _semaphore.WaitAsync();
        try
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText =
                $"DELETE FROM CARDINDEV cd " +
                $"WHERE cd.ID_CARDINDEV = @id";
            command.Parameters.Add("@id", id);
            await command.ExecuteNonQueryAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }
    public async Task IncrementAttemptsAsync(CardInDevGetListRow cardInDevGetListRow)
    {
        await _semaphore.WaitAsync();
        try
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText =
                $"UPDATE CARDINDEV " +
                $"SET ATTEMPTS = @id " +
                $"WHERE (ID_CARDINDEV = @attempts) AND (ID_DB = 1)";
            command.Parameters.Add("@id", cardInDevGetListRow.DeviceId);
            command.Parameters.Add("@attempts", cardInDevGetListRow.Attempts + 1);
            await command.ExecuteNonQueryAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

}
