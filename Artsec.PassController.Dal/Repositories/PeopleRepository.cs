using Artsec.PassController.Dal.Models;
using FirebirdSql.Data.FirebirdClient;

namespace Artsec.PassController.Dal.Repositories;

public class PeopleRepository
{
    private readonly IConnectionProvider _connectionProvider;

    public PeopleRepository(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<IEnumerable<PeopleModel>> GetAsync()
    {
        var result = new List<PeopleModel>();
        var sql =
            "select id_pep, authmode " +
            "from people ";

        using var connection = _connectionProvider.CreateConnection();
        using var command = new FbCommand(sql, connection);
        connection.Open();

        using var reader = await command.ExecuteReaderAsync();
        if (reader.HasRows)
        {
            while (await reader.ReadAsync())
            {
                result.Add(new()
                {
                    AuthMode = (int?)reader["AUTHMODE"],
                    PeopleId = (int)reader["ID_PEP"],
                });
            }
        }
        return result;
    }

    public async Task<PeopleModel?> GetByIdAsync(int id)
    {
        PeopleModel? result = null;
        var sql =
            "select id_pep, authmode " +
            "from people " +
            "WHERE id_pep = @Id";

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
                AuthMode = (int?)reader["AUTHMODE"],
                PeopleId = (int)reader["ID_PEP"],
            };
        }
        return result;
    }
}
