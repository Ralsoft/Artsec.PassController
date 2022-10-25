using Artsec.PassController.Dal.Base;
using Artsec.PassController.Dal.Models;
using Artsec.PassController.Dal.Repositories;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;

namespace Artsec.PassController.Dal;

public class PassControllerDbContext : DapperDbContextBase
{
    public Procedures Procedures { get; }
    public CardRepository Cards { get; }
    public PeopleRepository People { get; }
    public DeviceRepository Devices { get; }
    public PassControllerDbContext(IConnectionProvider connectionProvider) : base()
    {
        Procedures = new Procedures(connectionProvider);
        Cards = new CardRepository(connectionProvider);
        People = new PeopleRepository(connectionProvider);
        Devices = new DeviceRepository(connectionProvider);
    }
    protected override void ConfigureModel()
    {
        AddModel<CardModel>();
        AddModel<PeopleModel>();
        AddModel<DeviceModel>();
    }
}
