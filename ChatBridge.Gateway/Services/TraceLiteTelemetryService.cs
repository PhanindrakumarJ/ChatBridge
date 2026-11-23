namespace ChatBridge.Gateway.Services
{
    public class TraceLiteTelemetryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TraceLiteTelemetryService> _logger;

        public TraceLiteTelemetryService(
            HttpClient httpClient,
            ILogger<TraceLiteTelemetryService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task SendTelemetryAsync(object telemetryData)
        {
            try
            {
                HttpResponseMessage response =
                    await _httpClient.PostAsJsonAsync(
                        "/api/telemetry",
                        telemetryData);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "TraceLite rejected telemetry. Status: {StatusCode}",
                        (int)response.StatusCode);
                }
            }
            catch (Exception exception)
            {
                // Telemetry failures must not affect Gateway operations.
                _logger.LogWarning(
                    exception,
                    "Unable to send telemetry to TraceLite.");
            }
        }
    }
}
