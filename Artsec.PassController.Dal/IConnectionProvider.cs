namespace Artsec.PassController.Dal;

public interface IConnectionProvider
{
    SkudDbConnection CreateConnection();
    int ReservedConnectionsCount { get; }
}