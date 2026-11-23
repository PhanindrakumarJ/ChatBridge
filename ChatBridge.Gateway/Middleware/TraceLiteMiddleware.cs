
using System.Diagnostics;
using ChatBridge.Gateway.Services;

namespace ChatBridge.Gateway.Middleware
{
    public class TraceLiteMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TraceLiteMiddleware> _logger;

        public TraceLiteMiddleware(
            RequestDelegate next,
            ILogger<TraceLiteMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            TraceLiteTelemetryQueue telemetryQueue)
        {
            string requestId = context.Request.Headers["X-Request-ID"]
                .FirstOrDefault()
                ?? Guid.NewGuid().ToString();

            context.Request.Headers["X-Request-ID"] = requestId;
            context.Response.Headers["X-Request-ID"] = requestId;

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                try
                {
                    object telemetryData = new
                    {
                        application = "ChatBridge.Gateway",
                        eventType = "ApiRequest",
                        timestamp = DateTime.UtcNow.ToString("O"),
                        requestId = requestId,
                        data = new
                        {
                            method = context.Request.Method,
                            endpoint = context.Request.Path.Value ?? "/",
                            statusCode = context.Response.StatusCode,
                            durationMs = Math.Round(
                                stopwatch.Elapsed.TotalMilliseconds,
                                2)
                        }
                    };

                    bool isQueued =
                        telemetryQueue.TryEnqueue(telemetryData);

                    if (!isQueued)
                    {
                        _logger.LogWarning(
                            "TraceLite queue is full. Telemetry dropped. RequestId: {RequestId}",
                            requestId);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(
                        exception,
                        "Failed to enqueue TraceLite telemetry. RequestId: {RequestId}",
                        requestId);
                }
            }
        }
    }
}
