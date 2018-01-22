using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Lykke.MatchingEngine.Connector.Services;
using Newtonsoft.Json;
using Xunit;

namespace Lykke.MatchingEngine.Connector.Tests
{
    public class MeTests
    {
        [Theory(Skip = "integration test")]
        [InlineData("")]
        public async Task TransferTest(string meUrl)
        {
            var client = new TcpMatchingEngineClient(new IPEndPoint(IPAddress.Parse(Dns.GetHostAddresses(meUrl)[0].ToString()), 8888));
            client.Start();

            await Task.Delay(500);
            var result = await client.TransferAsync(Guid.NewGuid().ToString(), "a26d8644-6be9-4b26-bdc8-4046aa65ba86", "4d58c448-2c34-4804-a260-533d8efc9eef", "USD", 100, "e3fa1d1e-8e7a-44e0-a666-a442bc35515c", 0, 0);

            Assert.NotNull(result);

            Trace.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
