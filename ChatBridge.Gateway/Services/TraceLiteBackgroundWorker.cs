
using Microsoft.Extensions.Hosting;

namespace ChatBridge.Gateway.Services
{
    public class TraceLiteBackgroundWorker : BackgroundService
    {
        private readonly TraceLiteTelemetryQueue _telemetryQueue;
        private readonly TraceLiteTelemetryService _telemetryService;
        private readonly ILogger<TraceLiteBackgroundWorker> _logger;

        public TraceLiteBackgroundWorker(
            TraceLiteTelemetryQueue telemetryQueue,
            TraceLiteTelemetryService telemetryService,
            ILogger<TraceLiteBackgroundWorker> logger)
        {
            _telemetryQueue = telemetryQueue;
            _telemetryService = telemetryService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            await foreach (
                object telemetryData in
                _telemetryQueue.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await _telemetryService.SendTelemetryAsync(
                        telemetryData);
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(
                        exception,
                        "Failed to process TraceLite telemetry event.");
                }
            }
        }
    }
}
