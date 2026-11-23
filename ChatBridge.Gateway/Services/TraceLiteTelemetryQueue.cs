
using System.Threading.Channels;

namespace ChatBridge.Gateway.Services
{
    public class TraceLiteTelemetryQueue
    {
        private readonly Channel<object> _channel =
            Channel.CreateBounded<object>(
                new BoundedChannelOptions(1000)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = true,
                    SingleWriter = false
                });

        public bool TryEnqueue(object telemetryData)
        {
            return _channel.Writer.TryWrite(telemetryData);
        }

        public IAsyncEnumerable<object> ReadAllAsync(
            CancellationToken cancellationToken)
        {
            return _channel.Reader.ReadAllAsync(cancellationToken);
        }
    }
}
