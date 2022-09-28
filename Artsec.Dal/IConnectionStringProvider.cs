namespace Artsec.DAL;

public interface IConnectionStringProvider
{
    string ConnectionString { get; }
    void SaveConnectionString(string connectionString);
}
