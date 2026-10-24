
using Microsoft.Data.SqlClient;

namespace ChatBridge.Chat.Infrastructure.Resilience;

public static class GetRetryPolicy
{
    private const int MaxRetries = 3;

    public static async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        for (var attempt = 0; ; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                if(attempt< 2)
                {

                    //Included the If Conditin for cross check and lerning purpose of retry mechanism.

                    cancellationToken.ThrowIfCancellationRequested();

                    //Console output for first 2 tries.
                    Console.WriteLine(attempt);

                }
                else
                {
                    return await operation(cancellationToken);
                }
            }
            catch (SqlException ex) when (
                attempt < MaxRetries &&
                IsTransient(ex))
            {
                var delay = TimeSpan.FromSeconds(
                    Math.Pow(2, attempt + 1));

                await Task.Delay(delay, cancellationToken);
            }
        }
    }

    private static bool IsTransient(SqlException exception)
    {
        // SQL Server error numbers commonly associated
        // with transient connectivity or availability issues.
        return exception.Errors
            .Cast<SqlError>()
            .Any(error => error.Number is
                -2 or       // Command timeout
                2 or        // Network/server connection issue
                53 or       // Server not found
                64 or       // Network name no longer available
                233 or      // Connection initialization issue
                1205 or     // Deadlock
                10053 or    // Connection aborted
                10054 or    // Connection reset
                10060 or    // Connection timeout
                10928 or    // Azure SQL resource limit
                10929 or    // Azure SQL resource limit
                40197 or    // Azure SQL transient service error
                40501 or    // Azure SQL service busy
                40613 or    // Azure SQL database unavailable
                49918 or    // Azure SQL insufficient resources
                49919 or
                49920);
    }
}
