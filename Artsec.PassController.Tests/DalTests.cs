using Artsec.PassController.Dal;
using Artsec.PassController.Dal.Models;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Text;

namespace Artsec.PassController.Tests;

public class DalTests
{
    private const string ConnectionString =
            "User = SYSDBA; Password = temp; Database = C:\\Program Files (x86)\\Cardsoft\\DuoSE\\Access\\ACTUALDB.GDB; DataSource = 127.0.0.1; Port = 3050; Dialect = 3; Charset = win1251; Role =; Connection lifetime = 15; Pooling = true; MinPoolSize = 0; MaxPoolSize = 50; Packet Size = 8192; ServerType = 0;";

    private Dictionary<string, string> inMemorySettings = new()
    {
        {"ConnectionStrings:DefaultConnection", ConnectionString},
    };

    private IConfiguration GetConfigs()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [Fact]
    public async Task GetCardsTest()
    {
        var connectionProvider = new DefaultConnectionProvider(GetConfigs());
        var dbContext = new PassControllerDbContext(connectionProvider);

        var result = await dbContext.Cards.GetAsync();

        Assert.NotNull(result);
    }
    [Fact]
    public async Task GetPeopleTest()
    {
        var connectionProvider = new DefaultConnectionProvider(GetConfigs());
        var dbContext = new PassControllerDbContext(connectionProvider);

        var result = await dbContext.People.GetAsync();

        Assert.NotNull(result);
    }
    [Fact]
    public async Task ValidatePassTest()
    {
        var connectionProvider = new DefaultConnectionProvider(GetConfigs());
        var dbContext = new PassControllerDbContext(connectionProvider);

        var result = await dbContext.Procedures.ValidatePass(843, "12345678");

        Assert.NotNull(result);
    }
    [Fact]
    public async Task DeviceTest()
    {
        var connectionProvider = new DefaultConnectionProvider(GetConfigs());
        var dbContext = new PassControllerDbContext(connectionProvider);

        var result = await dbContext.Devices.GetByIdAsync(5);

        Assert.NotNull(result);
    }
    [Fact]
    public async Task ConnectionsStessTest()
    {
        var connectionProvider = new DefaultConnectionProvider(GetConfigs());
        var results = new List<IEnumerable<PeopleModel>>();
        var connections = new List<FbConnection>();
        var dbContext = new PassControllerDbContext(connectionProvider);
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        //for (int i = 0; i < 100; i++)
        //{
        //    var con = connectionProvider.CreateConnection();
        //    connections.Add(con);
        //}
        //for (int i = 0; i < 100; i++)
        //{
        //    connections[i].Open();
        //}
        //results.Add(await dbContext.People.GetAsync());
        Parallel.For(0, 2, async i =>
        {
            var result = await dbContext.People.GetAsync();
            results.Add(result);
        });

        for (int i = 0; i < 100; i++)
        {
            results.Add(await dbContext.People.GetAsync());
            await Task.Delay(10);
        }

        //var tasks = Enumerable.Range(0, 100)
        //                    .Select(i => dbContext.People.GetAsync());
        //results = (await Task.WhenAll(tasks)).ToList();


        //Assert.NotNull(result);
    }
}