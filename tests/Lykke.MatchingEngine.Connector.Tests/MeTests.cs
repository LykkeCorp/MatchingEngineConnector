using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Logs;
using Lykke.MatchingEngine.Connector.Models.Api;
using Lykke.MatchingEngine.Connector.Models.Common;
using Lykke.MatchingEngine.Connector.Services;
using Xunit;
using Xunit.Abstractions;

namespace Lykke.MatchingEngine.Connector.Tests
{
    public class MeTests
    {
        private readonly ITestOutputHelper _log;
        private readonly TcpMatchingEngineClient _client;

        public MeTests(ITestOutputHelper log)
        {
            _log = log;
            var url = "";

            _client = new TcpMatchingEngineClient(new IPEndPoint(IPAddress.Parse(Dns.GetHostAddresses(url)[0].ToString()), 8888), EmptyLogFactory.Instance);
            Thread.Sleep(100);
            _client.Start();
            Thread.Sleep(100);
        }

        [Fact(Skip = "Manual testing")]
        public async Task TransferTest()
        {
            FeeModel fee = new FeeModel()
            {
                TargetClientId = "e3fa1d1e-8e7a-44e0-a666-a442bc35515c",
                Size = 0
            };

            await Task.Delay(500);
            var sw = new Stopwatch();
            const int cycles = 100;
            sw.Start();
            for (int i = 0; i < cycles; i++)
            {
                var result = await _client.TransferAsync(Guid.NewGuid().ToString(), "a26d8644-6be9-4b26-bdc8-4046aa65ba86", "4d58c448-2c34-4804-a260-533d8efc9eef", "USD", 2, 100, fee, 0);
                Assert.NotNull(result);

                result = await _client.TransferAsync(Guid.NewGuid().ToString(), "4d58c448-2c34-4804-a260-533d8efc9eef", "a26d8644-6be9-4b26-bdc8-4046aa65ba86", "USD", 2, 100, fee, 0);
                Assert.NotNull(result);

            }

            _log.WriteLine((sw.ElapsedMilliseconds / (double)cycles / 2).ToString());
        }

        [Fact(Skip = "Manual testing")]
        public async Task TransferAbsoluteFee()
        {
            var clientId = "";
            var feeClientId = "";
            var amountClientId = "";

            FeeModel fee = new FeeModel()
            {
                SourceClientId = null,
                TargetClientId = feeClientId,
                Size = 15,
                SizeType = FeeSizeType.ABSOLUTE,
                Type = FeeType.CLIENT_FEE,
                ChargingType = FeeChargingType.SUBTRACT_FROM_AMOUNT
            };

            var result = await _client.TransferAsync(Guid.NewGuid().ToString(), clientId, amountClientId, "USD", 2, 22, fee, 0);
        }

        [Fact(Skip = "Manual testing")]
        // revert fee to client
        // transfer (return) amount from amountClientId to amountClientId
        // transfer (return) fee from feeClientId to amountClientId
        public async Task TransferRevertFee()
        {
            var clientId = "";
            var feeClientId = "";
            var amountClientId = "";

            FeeModel fee = new FeeModel()
            {
                SourceClientId = feeClientId,
                TargetClientId = clientId,
                Size = 15,
                SizeType = FeeSizeType.ABSOLUTE,
                Type = FeeType.EXTERNAL_FEE
            };

            var result = await _client.TransferAsync(Guid.NewGuid().ToString(), amountClientId, clientId, "USD", 2, 7, fee, 0);
        }

        [Fact(Skip = "Manual testing")]
        public async Task TransferPercentageFee()
        {
            var clientId = "";
            var feeClientId = "";
            var amountClientId = "";

            FeeModel fee = new FeeModel
            {
                SourceClientId = null,
                TargetClientId = feeClientId,
                Size = 0.025,
                SizeType = FeeSizeType.PERCENTAGE,
                Type = FeeType.CLIENT_FEE,
                ChargingType = FeeChargingType.SUBTRACT_FROM_AMOUNT
            };

            var result = await _client.TransferAsync(Guid.NewGuid().ToString(), clientId, amountClientId, "USD", 2, 11, fee, 0);
        }

        [Fact(Skip = "Manual testing")]
        public async Task TransferNoFee()
        {
            var clientId = "";
            var amountClientId = "";

            var result = await _client.TransferAsync(Guid.NewGuid().ToString(), clientId, amountClientId, "USD", 2, 32, null, 0);
        }
    }
}
