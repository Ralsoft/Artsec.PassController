using Artsec.PassController.Dal.Models;
using Dapper;

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
        var sql =
            "select d.ID_DEV, d.ID_CTRL " +
            "FROM DEVICE d " +
            "WHERE d.ID_DEV = @Id";

        var parameters = new { Id = id };
        using var connection = _connectionProvider.CreateConnection();
        await connection.OpenAsync();
        var result = await connection.QueryFirstOrDefaultAsync<DeviceModel>(sql, parameters);
        return result;
    }
}
