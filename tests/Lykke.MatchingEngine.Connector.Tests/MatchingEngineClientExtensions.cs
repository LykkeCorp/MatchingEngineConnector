using System;
using System.Threading.Tasks;
using Lykke.MatchingEngine.Connector.Abstractions.Services;

namespace Lykke.MatchingEngine.Connector.Tests
{
    public static class MatchingEngineClientExtensions
    {
        public  static async Task<T> Retry<T>(this IMatchingEngineClient client, Func<IMatchingEngineClient, Task<T>> action,
            int times = 5, int waitPeriodInMs = 500)
        {
            var waitPeriod = TimeSpan.FromMilliseconds(waitPeriodInMs);

            for (var i = 0; i < times; i++)
            {
                if (client.IsConnected)
                {
                    return await action(client);
                }
                Console.WriteLine($"Matching Engine client is not connected. Retrying after {waitPeriod}...");
                await Task.Delay(waitPeriod);
            }

            throw new TimeoutException($"Connection to Matching Engine failed after {times} retries.");
        }
    }
}
