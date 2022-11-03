using Artsec.PassController;
using Artsec.PassController.Configs;
using Artsec.PassController.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using System.Reflection;
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
            int? retainedFileCountLimit = context.Configuration.GetSection("WorkerOptions").GetValue<int?>("retainedFileCountLimit");
            var path = AppDomain.CurrentDomain.BaseDirectory;
            logging.AddFile(
                pathFormat: $"{path}\\Logs\\Log.log",
                minimumLevel: LogLevel.Trace,
                retainedFileCountLimit: retainedFileCountLimit,
                outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff}\t-\t[{Level:u3}] {Message}{NewLine}{Exception}");
        })
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            config.AddJsonFile("connectionsSettings.json", optional: false, reloadOnChange: false);
            config.AddJsonFile("controllersSettings.json", optional: false, reloadOnChange: false);
        })
        .ConfigureServices((hostContext, services) =>
        {
            services.AddConfigurations(hostContext.Configuration);
            services.AddHostedService<Worker>();

            //IConfiguration configuration = hostContext.Configuration;
            //WorkerOptions options = configuration.GetSection("WorkerOptions").Get<WorkerOptions>();
            services.AddHttpClient();
            services.RemoveAll<IHttpMessageHandlerBuilderFilter>();

            services.AddMemoryCache();
            services.AddListeners();
            services.AddServices();
            services.AddPipelines();
            services.AddDal(hostContext.Configuration);

        })
        .Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine("Error while starting. " + ex.Message);
    Console.ReadLine();
}
