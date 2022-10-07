using Artsec.PassController.Dal.Models;
using Dapper;

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
        var sql =
            "select id_pep, authmode " +
            "from people ";

        using var connection = _connectionProvider.CreateConnection();
        var result = await connection.QueryAsync<PeopleModel>(
            sql
        );
        return result ?? Enumerable.Empty<PeopleModel>();
    }
}
