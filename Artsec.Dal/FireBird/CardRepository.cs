using Artsec.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Artsec.Dal.FireBird;

public class CardRepository : FireBirdCommandsExecutorBase
{
    public CardRepository(IConnectionStringProvider connectionStringProvider) : base(connectionStringProvider)
    {
    }

    public async Task<int> GetCardsCountForControllerAsync(int ctrlId)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText =
            "select count(distinct c.id_card) from card c " +
            "join access ac on c.id_accessname=ac.id_accessname " +
            "join device d on ac.id_dev=d.id_dev " +
            "join device d2 on d2.id_ctrl=d.id_ctrl " +
            "where d2.id_reader is null " +
            "and d2.ID_CTRL=@ctrlId " +
            "group by d2.id_dev";
        command.Parameters.Add("@ctrlId", ctrlId);
        object response = await command.ExecuteScalarAsync();
        int result = response is int ? (int)response : 0;
        return result;
    }
}
