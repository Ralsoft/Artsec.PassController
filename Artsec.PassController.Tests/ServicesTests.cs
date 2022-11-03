using Artsec.PassController.Dal;
using Artsec.PassController.Services;
using Artsec.PassController.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.PassController.Tests
{
    public class ServicesTests
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
        public async Task PersonServiceTest()
        {

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();


            var connectionProvider = new DefaultConnectionProvider(GetConfigs());
            var dbContext = new PassControllerDbContext(connectionProvider);

            IPersonService service = new PersonService(dbContext, serviceProvider.GetRequiredService<IMemoryCache>(), null);

            string identifier = "E54064001A";
            var warmUp = await service.GetPersonIdAsync("000001001A");

            var sw = new Stopwatch();
            sw.Restart();
            var result = await service.GetPersonIdAsync(identifier);
            sw.Stop();
            Debug.WriteLine($"First exec: {sw.ElapsedMilliseconds}");
            await Task.Delay(1000);
            sw.Restart();
            result = await service.GetPersonIdAsync(identifier);
            sw.Stop();
            Debug.WriteLine($"Cache exec: {sw.ElapsedMilliseconds}");
            await Task.Delay(1000);
            sw.Restart();
            result = await service.GetPersonIdAsync(identifier);
            sw.Stop();
            Debug.WriteLine($"Cache exec: {sw.ElapsedMilliseconds}");
            await Task.Delay(1000);
            sw.Restart();
            result = await service.GetPersonIdAsync(identifier);
            sw.Stop();
            Debug.WriteLine($"Cache exec: {sw.ElapsedMilliseconds}");
            await Task.Delay(1000);
            sw.Restart();
            result = await service.GetPersonIdAsync(identifier);
            sw.Stop();
            Debug.WriteLine($"Cache exec: {sw.ElapsedMilliseconds}");
            await Task.Delay(1000);
            sw.Restart();
            result = await service.GetPersonIdAsync(identifier);
            sw.Stop();
            Debug.WriteLine($"Cache exec: {sw.ElapsedMilliseconds}");

            Assert.NotNull(result);
        }
    }
}
