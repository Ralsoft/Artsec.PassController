using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Pipelines;
using Artsec.PassController.Services.Interfaces;

namespace Artsec.PassController
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IInputAggregator _inputAggregator;
        private readonly PassRequestPipeline _passRequestPipeline;

        public Worker(ILogger<Worker> logger, IInputAggregator inputAggregator, PassRequestPipeline passRequestPipeline)
        {
            _logger = logger;
            _inputAggregator = inputAggregator;
            _passRequestPipeline = passRequestPipeline;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _inputAggregator.InputReceived += OnInputReceived!;
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
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void OnInputReceived(object sender, PassRequestWithPersonId request)
        {
            _ = _passRequestPipeline.PushAsync(request);
        }
    }
}