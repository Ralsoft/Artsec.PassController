using Artsec.PassController.Configs;
using Artsec.PassController.Domain;
using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Listeners.Implementation;
using Artsec.PassController.Pipelines;
using Artsec.PassController.Services.Interfaces;

namespace Artsec.PassController
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IInputAggregator _inputAggregator;
        private readonly PassRequestPipeline _passRequestPipeline;
        private readonly WorkerConfigurations _configs;
        private readonly ControllerListener _controllerListener;
        private readonly ICommandSender _commandSender;

        public Worker(ILogger<Worker> logger, IInputAggregator inputAggregator, PassRequestPipeline passRequestPipeline, WorkerConfigurations configs,
            ControllerListener controllerListener, ICommandSender commandSender)
        {
            _logger = logger;
            _inputAggregator = inputAggregator;
            _passRequestPipeline = passRequestPipeline;
            _configs = configs;
            _controllerListener = controllerListener;
            _commandSender = commandSender;
        }

        private bool IsControllerOnline(Controller controller)
        {
            _logger.LogInformation($"{controller.Name} is online");
            controller.IsOnline = true;
            return controller.IsOnline;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _inputAggregator.InputReceived += OnInputReceived!;

            foreach (var controller in _configs.Controllers.Values)
            {
                if (controller.IsActive)
                {
                    IsControllerOnline(controller);
                }
                if ((controller.IsOnline) && (controller.IsActive))
                {
                    var package = new ArtonitSePackager(ArtonitSeCommand.WriteData);
                    package.AddArg(0x02);
                    package.AddArg(0x03);
                    package.AddArg(0x05);
                    package.AddIp(_controllerListener.Ip);
                    package.AddPort(_controllerListener.Port);

                    byte[] data = package.Pack();

                    _commandSender.SendCommandAsync(data, controller.Ip, controller.Port);
                }
            }



            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _inputAggregator.InputReceived -= OnInputReceived!;
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void OnInputReceived(object sender, PassRequestWithPersonId request)
        {
            _logger.LogInformation("Начата обработка запроса\n" + 
                $"FaceId: {request.FaceId}\n" +
                $"Rfid: {request.Rfid}\n" +
                $"PersonId: {request.PersonId}\n" +
                $"CreationTime: {request.CreationTime}\n" +
                $"AuthMode: {request.AuthMode}");
            try
            {
                _ = _passRequestPipeline.PushAsync(request);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.Message);
            }
        }
    }
}