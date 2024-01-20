using Artsec.PassController.Dal;
using Artsec.PassController.Dal.Models;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using Xunit.Abstractions;

namespace Artsec.PassController.Tests;

public class DalTests
{
    private const string ConnectionString =
            "User = SYSDBA; Password = masterkey; Database = C:\\Program Files (x86)\\Cardsoft\\SHIELDPRO.GDB; DataSource = 127.0.0.1; Port = 3050; Dialect = 3; Charset = win1251; Role =; Connection lifetime = 15; Pooling = true; MinPoolSize = 0; MaxPoolSize = 50; Packet Size = 8192; ServerType = 0;";

    private readonly Dictionary<string, string> _inMemorySettings = new()
    {
        {"ConnectionStrings:DefaultConnection", ConnectionString},
    };

    private readonly ITestOutputHelper _output;

    public DalTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private IConfiguration GetConfigs()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(_inMemorySettings)
            .Build();
    }

    [Fact]
    public async Task GetCardsTest()
    {
        var connectionProvider = new DefaultConnectionProvider(GetConfigs(), null);
        var dbContext = new PassControllerDbContext(connectionProvider);

        var result = await dbContext.Cards.GetAsync();

        Assert.NotNull(result);
    }
    [Fact]
    public async Task GetPeopleTest()
    {
        var connectionProvider = new DefaultConnectionProvider(GetConfigs(), null);
        var dbContext = new PassControllerDbContext(connectionProvider);

        var result = await dbContext.People.GetAsync();

        Assert.NotNull(result);
    }
    [Fact]
    public async Task ValidatePassTest()
    {
        var connectionProvider = new DefaultConnectionProvider(GetConfigs(), null);
        var dbContext = new PassControllerDbContext(connectionProvider);

        var result = await dbContext.Procedures.ValidatePass(843, "12345678");

        Assert.NotNull(result);
    }
    [Fact]
    public async Task DeviceTest()
    {
        var connectionProvider = new DefaultConnectionProvider(GetConfigs(), null);
        var dbContext = new PassControllerDbContext(connectionProvider);

        var result = await dbContext.Devices.GetByIdAsync(5);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task ConnectionCountTest()
    {
        var connectionProvider = new DefaultConnectionProvider(GetConfigs(), null);
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        for (int i = 0; i < 100; i++)
        {
            _output.WriteLine("Iteration: {0}", i);
            Console.WriteLine("Iteration: {0}", i);
            var connection1 = connectionProvider.CreateConnection();
            var connection2 = connectionProvider.CreateConnection();

            await connection1.OpenAsync();
            await connection2.OpenAsync();
        }
    }
    [Fact]
    public async Task ConnectionsStessTest()
    {
        var connectionProvider = new DefaultConnectionProvider(GetConfigs(), null);
        var results2 = new List<IEnumerable<PeopleModel>>();
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
        //Parallel.For(0, 2, async i =>
        //{
        //    var result = await dbContext.People.GetAsync();
        //    results.Add(result);
        //});

        var counter = 0;
        while (true)
        {
            var results = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                //results.Add(await dbContext.People.GetAsync());
                try
                {
                    //results.Add(await dbContext.People.GetAsync());
                    await dbContext.Cards.GetByIdAsync("000");
                    results.Add(await dbContext.Procedures.ValidatePass(1, "6063"));
                }
                catch (FbException)
                {
                    
                }
                catch(Exception ex)
                {
                    _output.WriteLine("Err: {0}, {1}", ex.GetType(), ex.Message);
                    Console.WriteLine("Err: {0}, {1}", ex.GetType(), ex.Message);
                    // ignored
                }

                // await Task.Delay(10);
            }
            counter++;
            _output.WriteLine("Iteration: {0}", counter);
            Console.WriteLine("Iteration: {0}", counter);
        }

        //var tasks = Enumerable.Range(0, 100)
        //                    .Select(i => dbContext.People.GetAsync());
        //results = (await Task.WhenAll(tasks)).ToList();


        //Assert.NotNull(result);
    }
}