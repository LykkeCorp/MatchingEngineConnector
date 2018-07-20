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
                AssetPairId = "BTCUSD",
                OrderAction = OrderAction.Buy,
                LowerLimitPrice = 450,
                LowerPrice = 500,
                UpperLimitPrice = 650,
                UpperPrice = 600,
                Volume = 0.001
            };

            // Check that valid request is accepted
            var response = await client.Retry(x => x.PlaceStopLimitOrderAsync(model));
            ValidateResponse(response, MeStatusCodes.Ok);

            // Check that resend results in duplicate
            response = await client.Retry(x => x.PlaceStopLimitOrderAsync(model));
            ValidateResponse(response, MeStatusCodes.Duplicate);

            // Check that sending only upper prices is fine
            model.Id = GenerateUid();
            model.LowerLimitPrice = null;
            model.LowerPrice = null;
            model.UpperLimitPrice = 650;
            model.UpperPrice = 600;

            response = await client.PlaceStopLimitOrderAsync(model);
            ValidateResponse(response, MeStatusCodes.Ok);

            // Check that sending only upper limit price is wrong
            model.Id = GenerateUid();
            model.LowerLimitPrice = null;
            model.LowerPrice = null;
            model.UpperLimitPrice = 650;
            model.UpperPrice = null;

            response = await client.PlaceStopLimitOrderAsync(model);
            ValidateResponse(response, MeStatusCodes.InvalidPrice);

            // Check that sending only upper price is wrong
            model.Id = GenerateUid();
            model.LowerLimitPrice = null;
            model.LowerPrice = null;
            model.UpperLimitPrice = null;
            model.UpperPrice = 600;

            response = await client.PlaceStopLimitOrderAsync(model);
            ValidateResponse(response, MeStatusCodes.InvalidPrice);

            // Check that sending only lower prices is fine
            model.Id = GenerateUid();
            model.LowerLimitPrice = 450;
            model.LowerPrice = 500;
            model.UpperLimitPrice = null;
            model.UpperPrice = null;

            response = await client.PlaceStopLimitOrderAsync(model);
            ValidateResponse(response, MeStatusCodes.Ok);

            // Check that sending only lower limit price is wrong
            model.Id = GenerateUid();
            model.LowerLimitPrice = 450;
            model.LowerPrice = null;
            model.UpperLimitPrice = null;
            model.UpperPrice = null;

            response = await client.PlaceStopLimitOrderAsync(model);
            ValidateResponse(response, MeStatusCodes.InvalidPrice);

            // Check that sending only lower price is wrong
            model.Id = GenerateUid();
            model.LowerLimitPrice = null;
            model.LowerPrice = 500;
            model.UpperLimitPrice = null;
            model.UpperPrice = null;

            response = await client.PlaceStopLimitOrderAsync(model);
            ValidateResponse(response, MeStatusCodes.InvalidPrice);

            // Check that missing price gives InvalidPrice
            model.Id = GenerateUid();
            model.LowerLimitPrice = null;
            model.LowerPrice = null;
            model.UpperLimitPrice = null;
            model.UpperPrice = null;

            response = await client.PlaceStopLimitOrderAsync(model);
            ValidateResponse(response, MeStatusCodes.InvalidPrice);

            // Check that negative prices gives InvalidPrice
            model.Id = GenerateUid();
            model.LowerLimitPrice = -1;
            model.LowerPrice = -1;
            model.UpperLimitPrice = -1;
            model.UpperPrice = -1;

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