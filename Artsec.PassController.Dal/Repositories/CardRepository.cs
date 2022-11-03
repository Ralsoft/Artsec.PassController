using Artsec.PassController.Dal.Models;
using FirebirdSql.Data.FirebirdClient;

namespace Artsec.PassController.Dal.Repositories;

public class CardRepository
{
    private readonly IConnectionProvider _connectionProvider;

    public CardRepository(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<IEnumerable<CardModel>> GetAsync()
    {
        var result = new List<CardModel>();
        var sql =
            "select c.id_card, c.id_cardtype, p.id_pep, p.authmode " +
            "from card c join people p on p.id_pep = c.id_pep ";

        using var connection = _connectionProvider.CreateConnection();
        using var command = new FbCommand(sql, connection);
        connection.Open();

        using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();
        if (reader.HasRows)
        {
            while (await reader.ReadAsync())
            {
                result.Add(new()
                {
                    CardId = (string)reader["ID_CARD"],
                    CardTypeId = (int)reader["ID_CARDTYPE"],
                    PeopleId = (int)reader["ID_PEP"],
                    People = new()
                    {
                        PeopleId = (int)reader["ID_PEP"],
                        AuthMode = (int?)reader["AUTHMODE"],
                    }
                });
            }
        }
        return result;
    }
    public async Task<CardModel?> GetByIdAsync(string id)
    {
        CardModel? result = null;
        var sql =
            "select c.ID_CARD, c.ID_CARDTYPE, p.ID_PEP, p.AUTHMODE " +
            "from card c join people p on p.id_pep = c.id_pep " +
            "WHERE c.id_card = @Id";

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
                CardId = (string)reader["ID_CARD"],
                CardTypeId = (int)reader["ID_CARDTYPE"],
                PeopleId = (int)reader["ID_PEP"],
                People = new()
                {
                    PeopleId = (int)reader["ID_PEP"],
                    AuthMode = (int?)reader["AUTHMODE"],
                }
            };
        }
        return result;
    }
}
