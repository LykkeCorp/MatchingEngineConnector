using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Lykke.MatchingEngine.Connector.Services;
using Xunit;
using Xunit.Abstractions;

namespace Lykke.MatchingEngine.Connector.Tests
{
    public class MeTests
    {
        private readonly ITestOutputHelper _log;

        public MeTests(ITestOutputHelper log)
        {
            _log = log;
        }

        [Fact(Skip = "Manual testing")]
        public async Task TransferTest()
        {
            var url = "";
            var client = new TcpMatchingEngineClient(new IPEndPoint(IPAddress.Parse(Dns.GetHostAddresses(url)[0].ToString()), 8888));
            client.Start();

            await Task.Delay(500);
            var sw = new Stopwatch();
            const int cycles = 100;
            sw.Start();
            for (int i = 0; i < cycles; i++)
            {
                var result = await client.TransferAsync(Guid.NewGuid().ToString(), "a26d8644-6be9-4b26-bdc8-4046aa65ba86", "4d58c448-2c34-4804-a260-533d8efc9eef", "USD", 2, 100, "e3fa1d1e-8e7a-44e0-a666-a442bc35515c", 0, 0);
                Assert.NotNull(result);

                result = await client.TransferAsync(Guid.NewGuid().ToString(), "4d58c448-2c34-4804-a260-533d8efc9eef", "a26d8644-6be9-4b26-bdc8-4046aa65ba86", "USD", 2, 100, "e3fa1d1e-8e7a-44e0-a666-a442bc35515c", 0, 0);
                Assert.NotNull(result);

            }

            _log.WriteLine((sw.ElapsedMilliseconds / (double)cycles / 2).ToString());
        }
    }
}
