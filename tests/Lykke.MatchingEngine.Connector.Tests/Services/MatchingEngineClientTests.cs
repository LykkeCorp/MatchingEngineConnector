using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.MatchingEngine.Connector.Models.Api;
using Lykke.MatchingEngine.Connector.Models.Common;
using Xunit;

namespace Lykke.MatchingEngine.Connector.Tests.Services
{
    public class MatchingEngineClientTests : BaseMeClientTests
    {
        /// <summary>
        /// ME must return "NotFoundPrevious" when OldUid is specified and there are no orders with such Ids
        /// </summary>
        /// <returns></returns>
        [Fact(Skip = "Manual testing")]
        public async Task PlacingMultiLimitOrderDoesNotReplaceNotExistingOrders()
        {
            var client = CreateConnection(Url);

            // Try to replace not existing orders
            var multiOrder = CreateMultiOrder(ClientId, "LKKCHF", buyPrices: new[] { 0.1, 0.2 },
                oldBuyId: new[] { GenerateUid(), GenerateUid() });
            var response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiOrder));

            // Expecting error replacing not existing orders
            ValidateResponse(response, 2, MeStatusCodes.NotFoundPrevious);
        }

        /// <summary>
        /// ME must return "NotFoundPrevious" when OldUid is specified but the orders with specified Ids are already executed.
        /// </summary>
        /// <returns></returns>
        [Fact(Skip = "Manual testing")]
        public async Task PlacingMultiLimitOrderDoesNotReplaceExecutedOrders()
        {
            const string assetPairId = "LKKGBP";
            var clientIdSeller = "";

            Assert.NotEqual(clientIdSeller, ClientId);

            var client = CreateConnection(Url);

            // Place limit orders
            var multiOrderBuy = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.1, 0.2 });
            var buyResponse = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiOrderBuy));

            ValidateResponse(buyResponse, 2, MeStatusCodes.Ok);

            // Execute placed orders
            var multiOrderSell = CreateMultiOrder(clientIdSeller, assetPairId, sellPrices: new[] { 0.1, 0.1 });
            var sellResponse = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiOrderSell));

            ValidateResponse(sellResponse, 2, MeStatusCodes.Ok);

            // Try to replace executed orders
            var oldBuyId = buyResponse.Statuses.Select(s => s.Id).ToArray();
            var multiOrder = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.1, 0.2 }, oldBuyId: oldBuyId);
            var response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiOrder));

            // Expecting error on replacing not existing orders
            ValidateResponse(response, 2, MeStatusCodes.NotFoundPrevious);
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

            // Place 2 orders

            var multiOrder = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.1, 0.2 });
            var response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiOrder));

            ValidateResponse(response, 2, MeStatusCodes.Ok);

            // Replace both orders

            var nextMultiOrder = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.3, 0.4 },
                oldBuyId: response.Statuses.Select(s => s.Id).ToArray());
            response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(nextMultiOrder));

            ValidateResponse(response, 2, MeStatusCodes.Ok);

            // Replace 1 order and place 1 new order with cancel flag equal false
            // Expecting 3 existing orders

            var multiOrderNoReplace = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.1, 0.2 },
                oldBuyId: response.Statuses.Select(s => s.Id).Take(1).ToArray(), cancelPreviousOrders: false);
            response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiOrderNoReplace));

            ValidateResponse(response, 2, MeStatusCodes.Ok);
        }

        [Fact(Skip = "Manual testing")]
        public async Task PlaceMultiOrderReplacesOnlyOneSideWhenModeIsNotEmptySide()
        {
            const string assetPairId = "LKKGBP";
            var client = CreateConnection(Url);

            // Place buy and sell orders

            var multiOrder = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.1, 0.2 }, sellPrices: new[] { 1001.0, 1002.0 });
            var response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiOrder));

            ValidateResponse(response, 4, MeStatusCodes.Ok);

            // Replace only buy orders with Mode = NotEmptySide

            var multiBuyOrder = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.3, 0.4 });
            var buyResponse = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiBuyOrder));

            ValidateResponse(buyResponse, 2, MeStatusCodes.Ok);

            // Expecting sell orders still exist so they can be replaced

            var sellOrderIds = multiOrder.Orders.Where(o => o.OrderAction == OrderAction.Sell).Select(o => o.Id).ToArray();
            var multiSellOrder = CreateMultiOrder(ClientId, assetPairId, sellPrices: new[] { 1001.0, 1002.0 }, oldSellId: sellOrderIds);
            var sellResponse = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiSellOrder));

            ValidateResponse(sellResponse, 2, MeStatusCodes.Ok);
        }

        [Fact(Skip = "Manual testing")]
        public async Task PlaceMultiOrderReplacesOnlySellSideWhenModeIsSellSide()
        {
            const string assetPairId = "LKKGBP";
            var client = CreateConnection(Url);

            // Place buy and sell orders

            var multiOrder = CreateMultiOrder(ClientId, assetPairId,
                buyPrices: new[] { 0.1, 0.2 },
                sellPrices: new[] { 1001.0, 1002.0 });
            var response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiOrder));

            ValidateResponse(response, 4, MeStatusCodes.Ok);

            // Place new buy and sell orders with Mode=SellSide

            var replaceOrder = CreateMultiOrder(ClientId, assetPairId,
                buyPrices: new[] { 0.3 },
                sellPrices: new[] { 1003.0 }, cancelMode: CancelMode.SellSide);
            var replaceResponse = await client.Retry(c => c.PlaceMultiLimitOrderAsync(replaceOrder));

            ValidateResponse(replaceResponse, 2, MeStatusCodes.Ok);

            // Expecting old buy orders to exist so they can be replaced

            var buyOrderIds = multiOrder.Orders.Where(o => o.OrderAction == OrderAction.Buy).Select(o => o.Id).ToArray();
            var multiBuyOrder = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.1, 0.2 }, oldBuyId: buyOrderIds);
            var buyResponse = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiBuyOrder));

            ValidateResponse(buyResponse, 2, MeStatusCodes.Ok);

            // Expecting old sell orders to be canceled

            var sellOrderIds = multiOrder.Orders.Where(o => o.OrderAction == OrderAction.Sell).Select(o => o.Id).ToArray();
            var multiSellOrder = CreateMultiOrder(ClientId, assetPairId, sellPrices: new[] { 1001.0, 1002.0 }, oldSellId: sellOrderIds);
            var sellResponse = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiSellOrder));

            ValidateResponse(sellResponse, 2, MeStatusCodes.NotFoundPrevious);
        }

        [Fact(Skip = "Manual testing")]
        public async Task PlaceMultiOrderReplacesOnlyBuySideWhenModeIsBuySide()
        {
            const string assetPairId = "LKKGBP";
            var client = CreateConnection(Url);

            // Place buy and sell orders

            var multiOrder = CreateMultiOrder(ClientId, assetPairId,
                buyPrices: new[] { 0.1, 0.2 },
                sellPrices: new[] { 1001.0, 1002.0 });
            var response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiOrder));

            ValidateResponse(response, 4, MeStatusCodes.Ok);

            // Place new buy and sell orders with Mode=BuySide

            var replaceOrder = CreateMultiOrder(ClientId, assetPairId,
                buyPrices: new[] { 0.3 },
                sellPrices: new[] { 1003.0 }, cancelMode: CancelMode.BuySide);
            var replaceResponse = await client.Retry(c => c.PlaceMultiLimitOrderAsync(replaceOrder));

            ValidateResponse(replaceResponse, 2, MeStatusCodes.Ok);

            // Expecting old sell orders to exist so they can be replaced

            var sellOrderIds = multiOrder.Orders.Where(o => o.OrderAction == OrderAction.Sell).Select(o => o.Id).ToArray();
            var multiSellOrder = CreateMultiOrder(ClientId, assetPairId, sellPrices: new[] { 1001.0, 1002.0 }, oldSellId: sellOrderIds);
            var sellResponse = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiSellOrder));

            ValidateResponse(sellResponse, 2, MeStatusCodes.Ok);

            // Expecting old buy orders to be canceled

            var buyOrderIds = multiOrder.Orders.Where(o => o.OrderAction == OrderAction.Buy).Select(o => o.Id).ToArray();
            var multiBuyOrder = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.1, 0.2 }, oldBuyId: buyOrderIds);
            var buyResponse = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiBuyOrder));

            ValidateResponse(buyResponse, 2, MeStatusCodes.NotFoundPrevious);
        }

        [Fact(Skip = "Manual testing")]
        public async Task PlaceMultiOrderReplacesBothSidesWhenModeIsBothSides()
        {
            const string assetPairId = "LKKGBP";
            var client = CreateConnection(Url);

            // Place buy and sell orders

            var multiOrder = CreateMultiOrder(ClientId, assetPairId,
                buyPrices: new[] { 0.1, 0.2 },
                sellPrices: new[] { 1001.0, 1002.0 });
            var response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiOrder));

            ValidateResponse(response, 4, MeStatusCodes.Ok);

            // Place new buy and sell orders with Mode=BothSides

            var replaceOrder = CreateMultiOrder(ClientId, assetPairId,
                buyPrices: new[] { 0.3 },
                sellPrices: new[] { 1003.0 }, cancelMode: CancelMode.BothSides);
            var replaceResponse = await client.Retry(c => c.PlaceMultiLimitOrderAsync(replaceOrder));

            ValidateResponse(replaceResponse, 2, MeStatusCodes.Ok);

            // Expecting old sell orders to be canceled

            var sellOrderIds = multiOrder.Orders.Where(o => o.OrderAction == OrderAction.Sell).Select(o => o.Id).ToArray();
            var multiSellOrder = CreateMultiOrder(ClientId, assetPairId, sellPrices: new[] { 1001.0, 1002.0 }, oldSellId: sellOrderIds);
            var sellResponse = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiSellOrder));

            ValidateResponse(sellResponse, 2, MeStatusCodes.NotFoundPrevious);

            // Expecting old buy orders to be canceled

            var buyOrderIds = multiOrder.Orders.Where(o => o.OrderAction == OrderAction.Buy).Select(o => o.Id).ToArray();
            var multiBuyOrder = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.1, 0.2 }, oldBuyId: buyOrderIds);
            var buyResponse = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiBuyOrder));

            ValidateResponse(buyResponse, 2, MeStatusCodes.NotFoundPrevious);
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
                oldBuyId: response.Statuses.Select(s => s.Id).ToArray());
            response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(nextMultiOrder));

            // Expecting error as the orders do not exist already

            ValidateResponse(response, 2, MeStatusCodes.NotFoundPrevious);
        }

        [Fact(Skip = "Manual testing")]
        public async Task CancelLimitOrderTest()
        {
            const string assetPairId = "LKKCHF";

            var client = CreateConnection(Url);

            // Place orders

            var multiOrder = CreateMultiOrder(ClientId, assetPairId, buyPrices: new[] { 0.1, 0.2 });
            var response = await client.Retry(c => c.PlaceMultiLimitOrderAsync(multiOrder));

            ValidateResponse(response, 2, MeStatusCodes.Ok);

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

            ValidateResponse(response, 2, MeStatusCodes.Ok);

            // Cancel all orders at once

            var cancelResponse = await client.Retry(c => c.CancelLimitOrdersAsync(
                multiOrder.Orders.Select(o => o.Id)));

            Assert.NotNull(cancelResponse);
            Assert.Equal(MeStatusCodes.Ok, cancelResponse.Status);
        }

        private static MultiLimitOrderModel CreateMultiOrder(string clientId, string assetPairId,
            double[] buyPrices = null, double[] sellPrices = null, string[] oldBuyId = null, string[] oldSellId = null,
            bool cancelPreviousOrders = true, CancelMode cancelMode = CancelMode.NotEmptySide)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId), "ClientId must be specified");
            }

            const int volume = 10;

            var orderItemsBuy = buyPrices?.Select((buyPrice, index) =>
                                    new MultiOrderItemModel
                                    {
                                        Id = GenerateUid(),
                                        OrderAction = OrderAction.Buy,
                                        Price = buyPrice,
                                        Volume = volume,
                                        OldId = oldBuyId != null && oldBuyId.Length > index ? oldBuyId[index] : null
                                    })
                                ?? Enumerable.Empty<MultiOrderItemModel>();

            var orderItemsSell = sellPrices?.Select((sellPrice, index) =>
                                     new MultiOrderItemModel
                                     {
                                         Id = GenerateUid(),
                                         OrderAction = OrderAction.Sell,
                                         Price = sellPrice,
                                         Volume = volume,
                                         OldId = oldSellId != null && oldSellId.Length > index ? oldSellId[index] : null
                                     })
                                 ?? Enumerable.Empty<MultiOrderItemModel>();

            return new MultiLimitOrderModel
            {
                AssetPairId = assetPairId,
                CancelPreviousOrders = cancelPreviousOrders,
                ClientId = clientId,
                Id = GenerateUid(),
                CancelMode = cancelMode,
                Orders = orderItemsBuy.Concat(orderItemsSell).ToList()
            };
        }

        [AssertionMethod]
        private static void ValidateResponse(MultiLimitOrderResponse response, int expectedCount,
            MeStatusCodes expectedStatusCode)
        {
            Assert.NotNull(response);
            Assert.Equal(expectedCount, response.Statuses.Count);
            Assert.True(response.Statuses.All(s => s.Status == expectedStatusCode));
        }
    }
}
