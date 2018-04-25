using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Services;
using Xunit;
using Xunit.Abstractions;

namespace Lykke.MatchingEngine.Connector.Tests
{
    public class MeTests
    {
        private const string Url = "";
        private const string ClientId = "";

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
                var result = await client.TransferAsync(Guid.NewGuid().ToString(), "a26d8644-6be9-4b26-bdc8-4046aa65ba86", "4d58c448-2c34-4804-a260-533d8efc9eef", "USD", 2, 100, fee, 0);
                Assert.NotNull(result);

                result = await client.TransferAsync(Guid.NewGuid().ToString(), "4d58c448-2c34-4804-a260-533d8efc9eef", "a26d8644-6be9-4b26-bdc8-4046aa65ba86", "USD", 2, 100, fee, 0);
                Assert.NotNull(result);

            }

            _log.WriteLine((sw.ElapsedMilliseconds / (double)cycles / 2).ToString());
        }

        [Fact(Skip = "Manual testing")]
        public async Task WithdrawalFeeTest()
        {
            var url = "";
            var client = new TcpMatchingEngineClient(new IPEndPoint(IPAddress.Parse(Dns.GetHostAddresses(url)[0].ToString()), 8888));
            client.Start();

            FeeModel fee = new FeeModel()
            {
                SourceClientId = "e3fa1d1e-8e7a-44e0-a666-a442bc35515c",
                TargetClientId = "",
                Size = 15,
                SizeType = FeeSizeType.ABSOLUTE,
            };


            var result = await client.TransferAsync(Guid.NewGuid().ToString(), "", "", "USD", 2, 14, fee, 0);

        }

        /// <summary>
        /// When OldUid is specified and there are no orders with such Ids then Placing returns error
        /// </summary>
        /// <returns></returns>
        [Fact(Skip = "Manual testing")]
        public async Task PlacingMultiLimitOrderDoesNotReplaceNotExistingOrders()
        {
            var client = CreateConnection(Url);

            // Try to replace not existing orders

            var multiOrder = CreateMultiOrder(ClientId, "LKKCHF", buyPrices: new[] {0.1, 0.2}, oldId: new[] {GenerateUid(), GenerateUid()});

            var response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiOrder));

            Assert.NotNull(response);
            Assert.Equal(2, response.Statuses.Count);
            Assert.True(response.Statuses.All(s => s.Status == MeStatusCodes.NotFoundPrevious));
        }

        /// <summary>
        /// When OldUid is specified, new orders replace old orders
        /// </summary>
        /// <returns></returns>
        [Fact(Skip = "Manual testing")]
        public async Task PlacingMultiLimitOrderReplacesSpecifiedOrders()
        {
            const string assetPairId = "LKKCHF";

            var client = CreateConnection(Url);

            // Place orders

            var multiOrder = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.1, 0.2 });

            var response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiOrder));

            Assert.NotNull(response);
            Assert.Equal(2, response.Statuses.Count);
            Assert.True(response.Statuses.All(s => s.Status == MeStatusCodes.Ok));

            // Replace orders

            var nextMultiOrder = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.3, 0.4 },
                oldId: response.Statuses.Select(s => s.Id).ToArray());

            response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(nextMultiOrder));

            Assert.NotNull(response);
            Assert.Equal(2, response.Statuses.Count);
            Assert.True(response.Statuses.All(s => s.Status == MeStatusCodes.Ok));

            // Replace orders when cancel flag is false

            var multiOrderNoReplace = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.1, 0.2 },
                oldId: response.Statuses.Select(s => s.Id).Take(1).ToArray(), cancelPreviousOrders: false);

            response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiOrderNoReplace));

            Assert.NotNull(response);
            Assert.Equal(2, response.Statuses.Count);
            Assert.True(response.Statuses.All(s => s.Status == MeStatusCodes.Ok));
        }

        [Fact(Skip = "Manual testing")]
        public async Task MassCancelOrderTest()
        {
            const string assetPairId = "LKKCHF";

            var client = CreateConnection(Url);

            // Place orders

            var multiOrder = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.1, 0.2 });

            var response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiOrder));

            Assert.NotNull(response);
            Assert.Equal(2, response.Statuses.Count);
            Assert.True(response.Statuses.All(s => s.Status == MeStatusCodes.Ok));

            // Cancel all orders

            var cancelResponse = await client.MassCancelLimitOrdersAsync(new LimitOrderMassCancelModel
            {
                ClientId = ClientId,
                Id = GenerateUid(),
                AssetPairId = assetPairId,
                IsBuy = null
            });

            Assert.Equal(MeStatusCodes.Ok, cancelResponse.Status);

            // Try to replace orders

            var nextMultiOrder = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.3, 0.4 },
                oldId: response.Statuses.Select(s => s.Id).ToArray());

            response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(nextMultiOrder));

            Assert.NotNull(response);
            Assert.Equal(2, response.Statuses.Count);
            Assert.True(response.Statuses.All(s => s.Status == MeStatusCodes.NotFoundPrevious));
        }

        [Fact(Skip = "Manual testing")]
        public async Task CancelLimitOrderTest()
        {
            const string assetPairId = "LKKCHF";

            var client = CreateConnection(Url);

            // Place orders

            var multiOrder = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.1, 0.2 });

            var response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiOrder));

            Assert.NotNull(response);
            Assert.Equal(2, response.Statuses.Count);
            Assert.True(response.Statuses.All(s => s.Status == MeStatusCodes.Ok));

            // Cancel all orders one by one

            var tasks = multiOrder.Orders.Select(o => client.Retry(c => c.CancelLimitOrderAsync(o.Id))).ToList();
            await Task.WhenAll(tasks);

            foreach (var cancelResponse in tasks.Select(t => t.Result))
            {
                Assert.NotNull(cancelResponse);
                Assert.Equal(MeStatusCodes.Ok, cancelResponse.Status);
            }
        }

        [Fact(Skip = "Manual testing")]
        public async Task CancelLimitOrdersTest()
        {
            const string assetPairId = "LKKCHF";

            var client = CreateConnection(Url);

            // Place orders

            var multiOrder = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.1, 0.2 });

            var response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiOrder));

            Assert.NotNull(response);
            Assert.Equal(2, response.Statuses.Count);
            Assert.True(response.Statuses.All(s => s.Status == MeStatusCodes.Ok));

            // Cancel all orders one by one

            var cancelResponse = await client.Retry(c => c.CancelLimitOrdersAsync(
                multiOrder.Orders.Select(o => o.Id).ToArray()));

            Assert.NotNull(cancelResponse);
            Assert.Equal(MeStatusCodes.Ok, cancelResponse.Status);
        }

        private static MultiLimitOrderModel CreateMultiOrder(string clientId, string assetPairId, double[] buyPrices, string[] oldId = null,
            bool cancelPreviousOrders = true)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId), "ClientId must be specified");
            }

            const int volume = 10;

            var orderItems = buyPrices.Select((buyPrice, index) =>
                new MultiOrderItemModel
                {
                    Id = GenerateUid(),
                    OrderAction = OrderAction.Buy,
                    Price = buyPrice,
                    Volume = volume,
                    OldId = oldId != null && oldId.Length > index ? oldId[index] : null
                }).ToArray();

            return new MultiLimitOrderModel
            {
                AssetId = assetPairId,
                CancelPreviousOrders = cancelPreviousOrders,
                ClientId = clientId,
                Id = GenerateUid(),
                CancelMode = CancelMode.NotEmptySide,
                Orders = orderItems
            };
        }

        private static string GenerateUid()
        {
            return Guid.NewGuid().ToString();
        }

        private static IMatchingEngineClient CreateConnection(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url), "Url must be specified");
            }

            var client =
                new TcpMatchingEngineClient(new IPEndPoint(IPAddress.Parse(Dns.GetHostAddresses(url)[0].ToString()),
                    8888));
            client.Start();
            return client;
        }
    }
}
