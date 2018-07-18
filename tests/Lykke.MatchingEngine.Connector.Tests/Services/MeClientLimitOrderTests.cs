using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.MatchingEngine.Connector.Models.Api;
using Lykke.MatchingEngine.Connector.Models.Common;
using Xunit;

namespace Lykke.MatchingEngine.Connector.Tests.Services
{
    public class MeClientLimitOrderTests : BaseMeClientTests
    {
        [Fact(Skip = "Manual testing")]
        public async Task PlaceRegularLimitOrder()
        {
            var client = CreateConnection(Url);

            var model = new LimitOrderModel
            {
                Id = GenerateUid(),
                ClientId = ClientId,
                CancelPreviousOrders = true,
                AssetPairId = "LKKCHF",
                OrderAction = OrderAction.Buy,
                Price = 0.005,
                Volume = 10
            };

            // Check that valid request is accepted
            var response = await client.Retry(x => x.PlaceLimitOrderAsync(model));
            ValidateResponse(response, MeStatusCodes.Ok);

            // Check that resend results in duplicate
            response = await client.Retry(x => x.PlaceLimitOrderAsync(model));
            ValidateResponse(response, MeStatusCodes.Duplicate);

            // Check that missing price gives InvalidPrice
            model.Id = GenerateUid();
            model.Price = 0;

            response = await client.Retry(x => x.PlaceLimitOrderAsync(model));
            ValidateResponse(response, MeStatusCodes.InvalidPrice);
        }

        [Fact(Skip = "Manual testing")]
        public async Task PlaceStopLimitOrder()
        {
            var client = CreateConnection(Url);

            var model = new StopLimitOrderModel
            {
                Id = GenerateUid(),
                ClientId = ClientId,
                CancelPreviousOrders = true,
                AssetPairId = "LKKCHF",
                OrderAction = OrderAction.Buy,
                LowerLimitPrice = 0.004,
                LowerPrice = 0.005,
                UpperLimitPrice = 0.007,
                UpperPrice = 0.006,
                Volume = 10
            };

            // Check that valid request is accepted
            var response = await client.Retry(x => x.PlaceStopLimitOrderAsync(model));
            ValidateResponse(response, MeStatusCodes.Ok);

            // Check that resend results in duplicate
            response = await client.Retry(x => x.PlaceStopLimitOrderAsync(model));
            ValidateResponse(response, MeStatusCodes.Duplicate);

            // Check that missing price gives InvalidPrice
            model.Id = GenerateUid();
            model.LowerLimitPrice = 0;
            model.LowerPrice = 0;
            model.UpperLimitPrice = 0;
            model.UpperPrice = 0;

            response = await client.PlaceStopLimitOrderAsync(model);
            ValidateResponse(response, MeStatusCodes.InvalidPrice);
        }

        [AssertionMethod]
        private static void ValidateResponse(MeResponseModel response, MeStatusCodes expected)
        {
            Assert.NotNull(response);
            Assert.Equal(response.Status, expected);
        }
    }
}