using Artsec.PassController.Dal;
using Artsec.PassController.Services;
using Artsec.PassController.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.PassController.Tests
{
    public class ServicesTests
    {
        private const string ConnectionString =
                "User = SYSDBA; Password = temp; Database = C:\\Program Files (x86)\\Cardsoft\\DuoSE\\Access\\ACTUALDB.GDB; DataSource = 127.0.0.1; Port = 3050; Dialect = 3; Charset = win1251; Role =; Connection lifetime = 15; Pooling = true; MinPoolSize = 0; MaxPoolSize = 50; Packet Size = 8192; ServerType = 0;";

        [Fact]
        public async Task PersonServiceTest()
        {

            var connectionProvider = new ConnectionProvider(ConnectionString);
            var dbContext = new PassControllerDbContext(connectionProvider);

            IPersonService service = new PersonService(dbContext);
            var result = await service.GetPersonIdAsync("15005");

            Assert.NotNull(result);
        }
    }
}
