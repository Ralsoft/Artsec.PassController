using Artsec.PassController.Dal;

namespace Artsec.PassController.Tests;

public class DalTests
{
    private const string ConnectionString =
            "User = SYSDBA; Password = temp; Database = C:\\Program Files (x86)\\Cardsoft\\DuoSE\\Access\\ACTUALDB.GDB; DataSource = 127.0.0.1; Port = 3050; Dialect = 3; Charset = win1251; Role =; Connection lifetime = 15; Pooling = true; MinPoolSize = 0; MaxPoolSize = 50; Packet Size = 8192; ServerType = 0;";

    [Fact]
    public async Task GetCardsTest()
    {
        var connectionProvider = new ConnectionProvider(ConnectionString);
        var dbContext = new PassControllerDbContext(connectionProvider);

        var result = await dbContext.Cards.GetAsync();

        Assert.NotNull(result);
    }
    [Fact]
    public async Task GetPeopleTest()
    {
        var connectionProvider = new ConnectionProvider(ConnectionString);
        var dbContext = new PassControllerDbContext(connectionProvider);

        var result = await dbContext.People.GetAsync();

        Assert.NotNull(result);
    }
    [Fact]
    public async Task ValidatePassTest()
    {
        var connectionProvider = new ConnectionProvider(ConnectionString);
        var dbContext = new PassControllerDbContext(connectionProvider);

        var result = await dbContext.Procedures.ValidatePass(843, "12345678");

        Assert.NotNull(result);
    }
}