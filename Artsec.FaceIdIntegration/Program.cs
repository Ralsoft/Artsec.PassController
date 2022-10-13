using Artsec.PassController;
using Artsec.PassController.Extensions;
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
            var path = Directory.GetCurrentDirectory();
            logging.AddFile(
                pathFormat: $"{path}\\Logs\\Log.log",
                minimumLevel: LogLevel.Trace,
                retainedFileCountLimit: retainedFileCountLimit,
                outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff}\t-\t[{Level:u3}] {Message}{NewLine}{Exception}");
        })
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<Worker>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //IConfiguration configuration = hostContext.Configuration;
            //WorkerOptions options = configuration.GetSection("WorkerOptions").Get<WorkerOptions>();
            services.AddHttpClient();
            services.AddListeners();
            services.AddServices();
            services.AddPipelines();
            services.AddDal(hostContext.Configuration);
            services.AddConfigurations(hostContext.Configuration);

        })
        .Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine("Error while starting. " + ex.Message);
    Console.ReadLine();
}
