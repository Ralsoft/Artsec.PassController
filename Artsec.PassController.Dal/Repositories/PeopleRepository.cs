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
        try
        {

            var sql =
                "select id_pep, authmode " +
                "from people ";

            using var connection = _connectionProvider.CreateConnection();
            await connection.OpenAsync();
            var result = await connection.QueryAsync<PeopleModel>(
                sql
            );
            return result ?? Enumerable.Empty<PeopleModel>();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<PeopleModel?> GetByIdAsync(int id)
    {
        var sql =
            "select id_pep, authmode " +
            "from people " +
            "WHERE id_pep = @Id";

        var parameters = new { Id = id };

        using var connection = _connectionProvider.CreateConnection();
        await connection.OpenAsync();
        var result = await connection.QueryFirstOrDefaultAsync<PeopleModel>(
            sql,
            param: parameters
        );
        return result;
    }
}
