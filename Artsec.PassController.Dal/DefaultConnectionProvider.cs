using System.Data;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Artsec.PassController.Dal;

public class DefaultConnectionProvider : IConnectionProvider
{
    private readonly ILoggerFactory? _loggerFactory;
    private readonly IConfiguration _configuration;
    private const string ConnectionStringName = "DefaultConnection";
    private int _connectionCount = 0;

    public DefaultConnectionProvider(IConfiguration configuration, ILoggerFactory? loggerFactory)
    {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
    }

    public SkudDbConnection CreateConnection()
    {
        var connection = new FbConnection(_configuration.GetConnectionString(ConnectionStringName));
        return new SkudDbConnection(connection, this, _loggerFactory?.CreateLogger<SkudDbConnection>());
    }

    public int ReservedConnectionsCount => _connectionCount;
    public int IncrementConnectionsCount() => Interlocked.Increment(ref _connectionCount);
    public int DecrementConnectionsCount() => Interlocked.Decrement(ref _connectionCount);
}

public class SkudDbConnection : IDbConnection, IAsyncDisposable
{
    private readonly DefaultConnectionProvider _connectionProvider;
    private readonly ILogger<SkudDbConnection>? _logger;

    public SkudDbConnection(
        FbConnection fbConnection, 
        DefaultConnectionProvider connectionProvider,
        ILogger<SkudDbConnection>? logger)
    {
        Connection = fbConnection;
        _connectionProvider = connectionProvider;
        _logger = logger;
        _connectionProvider.IncrementConnectionsCount();
    }

    public FbConnection Connection { get; }

    public void Dispose()
    {
        _connectionProvider.DecrementConnectionsCount();
        Connection.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        _connectionProvider.DecrementConnectionsCount();
        await Connection.DisposeAsync();
    }

    public Task OpenAsync()
    {
        try
        {
            return Connection.OpenAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Connections count: {ConnectionsCount}, ErrType: {ErrType}, ErrMessage: {ErrMessage}, StackTrace: {StackTrace}",
                _connectionProvider.ReservedConnectionsCount,
                ex,
                ex.Message,
                ex.StackTrace);
            throw;
        }
    }

    public IDbTransaction BeginTransaction()
    {
        return Connection.BeginTransaction();
    }

    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
        return Connection.BeginTransaction(il);
    }

    public void ChangeDatabase(string databaseName)
    {
        Connection.ChangeDatabase(databaseName);
    }

    public void Close()
    {
        Connection.Close();
    }

    IDbCommand IDbConnection.CreateCommand()
    {
        return Connection.CreateCommand();
    }

    public void Open()
    {
        Connection.Open();
    }

    public string ConnectionString
    {
        get => Connection.ConnectionString;
        set => Connection.ConnectionString = value;
    }

    public int ConnectionTimeout => Connection.ConnectionTimeout;
    public string Database => Connection.Database;
    public ConnectionState State => Connection.State;

    public FbCommand CreateCommand()
    {
        return Connection.CreateCommand();
    }
}