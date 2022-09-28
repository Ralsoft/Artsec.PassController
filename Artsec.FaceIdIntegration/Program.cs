using Artsec.PassController;
using Artsec.PassController.Listeners.Extensions;
using System.Net.Sockets;
using System.Text;

try
{
    IHost host = Host.CreateDefaultBuilder(args)
        .UseWindowsService(options =>
        {
            options.ServiceName = "Artonit Over2";
        })
        .ConfigureLogging((context, logging) =>
        {
            logging.ClearProviders();
            logging.AddConfiguration(context.Configuration.GetSection("Logging"));
            logging.AddConsole();
            string path = context.Configuration.GetSection("WorkerOptions").GetValue<string>("LogFolerPath");
            int? retainedFileCountLimit = context.Configuration.GetSection("WorkerOptions").GetValue<int?>("retainedFileCountLimit");
            LogLevel foo = context.Configuration.GetSection("Logging:LogLevel").GetValue<LogLevel>("Default");
            logging.AddFile(
                pathFormat: $"{path}\\Log.log",
                minimumLevel: LogLevel.Trace,
                retainedFileCountLimit: retainedFileCountLimit,
                outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff}\t-\t[{Level:u3}] {Message}{NewLine}{Exception}");
        })
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<Worker>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            IConfiguration configuration = hostContext.Configuration;
            //WorkerOptions options = configuration.GetSection("WorkerOptions").Get<WorkerOptions>();
            services.AddListeners();

        })
        .Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine("Error while starting. " + ex.Message);
}
