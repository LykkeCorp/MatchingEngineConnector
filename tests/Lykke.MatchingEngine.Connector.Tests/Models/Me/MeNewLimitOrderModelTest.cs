using System;
using Lykke.MatchingEngine.Connector.Models.Common;
using Lykke.MatchingEngine.Connector.Models.Me;
using Xunit;

namespace Lykke.MatchingEngine.Connector.Tests.Models.Me
{
    public class MeNewLimitOrderModelTest
    {
        private MeLimitOrderFeeModel CreateFee()
        {
            return new MeLimitOrderFeeModel
            {
                AssetId = new[] { "LKK" }
            };
        }

        [Fact]
        public void CreateRegularLimitOrderModel_Buy()
        {
            var id = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid().ToString();
            var assetPairId = "LKKEUR";
            var orderAction = OrderAction.Buy;
            var volume = 234.01;
            var price = 5432.5;
            var cancelAllPrevious = true;
            var fee = CreateFee();
            var fees = new[]
            {
                CreateFee(),
                CreateFee()
            };

            var model = MeNewLimitOrderModel.CreateMeLimitOrder(
                id,
                clientId,
                assetPairId,
                orderAction,
                volume,
                price,
                cancelAllPrevious,
                fee,
                fees
            );

            Assert.Equal(id, model.Id);
            Assert.Equal(clientId, model.ClientId);
            Assert.Equal(assetPairId, model.AssetPairId);
            Assert.Equal(volume, model.Volume);
            Assert.Equal(price, model.Price);
            Assert.Equal(cancelAllPrevious, model.CancelAllPreviousLimitOrders);
            Assert.Equal(fee, model.Fee);
            Assert.Equal(fees, model.Fees);
            Assert.Equal(0, model.Type);
        }

        [Fact]
        public void CreateRegularLimitOrderModel_Sell()
        {
            var id = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid().ToString();
            var assetPairId = "LKKEUR";
            var orderAction = OrderAction.Sell;
            var volume = 234.01;
            var price = 5432.5;
            var cancelAllPrevious = true;
            var fee = CreateFee();
            var fees = new[]
            {
                CreateFee(),
                CreateFee()
            };

            var model = MeNewLimitOrderModel.CreateMeLimitOrder(
                id,
                clientId,
                assetPairId,
                orderAction,
                volume,
                price,
                cancelAllPrevious,
                fee,
                fees
            );

            Assert.Equal(id, model.Id);
            Assert.Equal(clientId, model.ClientId);
            Assert.Equal(assetPairId, model.AssetPairId);
            Assert.Equal(-volume, model.Volume);
            Assert.Equal(price, model.Price);
            Assert.Equal(cancelAllPrevious, model.CancelAllPreviousLimitOrders);
            Assert.Equal(fee, model.Fee);
            Assert.Equal(fees, model.Fees);
            Assert.Equal(0, model.Type);
        }

        [Fact]
        public void CreateStopLimitOrderModel_Buy()
        {
            var id = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid().ToString();
            var assetPairId = "LKKEUR";
            var orderAction = OrderAction.Buy;
            var volume = 234.01;
            var lowerPrice = 5432.5;
            var lowerLimitPrice = 6432.5;
            var upperPrice = 5432.7;
            var upperLimitPrice = 5438.5;
            var cancelAllPrevious = true;
            var fee = CreateFee();
            var fees = new[]
            {
                CreateFee(),
                CreateFee()
            };

            var model = MeNewLimitOrderModel.CreateMeStopLimitOrder(
                id,
                clientId,
                assetPairId,
                orderAction,
                volume,
                cancelAllPrevious,
                fee,
                fees,
                lowerLimitPrice,
                lowerPrice,
                upperLimitPrice,
                upperPrice
            );

            Assert.Equal(id, model.Id);
            Assert.Equal(clientId, model.ClientId);
            Assert.Equal(assetPairId, model.AssetPairId);
            Assert.Equal(volume, model.Volume);
            Assert.Equal(cancelAllPrevious, model.CancelAllPreviousLimitOrders);
            Assert.Equal(fee, model.Fee);
            Assert.Equal(fees, model.Fees);
            Assert.Equal(1, model.Type);
            Assert.Equal(lowerLimitPrice, model.LowerLimitPrice);
            Assert.Equal(lowerPrice, model.LowerPrice);
            Assert.Equal(upperLimitPrice, model.UpperLimitPrice);
            Assert.Equal(upperPrice, model.UpperPrice);
        }

        [Fact]
        public void CreateStopLimitOrderModel_Sell()
        {
            var id = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid().ToString();
            var assetPairId = "LKKEUR";
            var orderAction = OrderAction.Sell;
            var volume = 234.01;
            var lowerPrice = 5432.5;
            var lowerLimitPrice = 6432.5;
            var upperPrice = 5432.7;
            var upperLimitPrice = 5438.5;
            var cancelAllPrevious = true;
            var fee = CreateFee();
            var fees = new[]
            {
                CreateFee(),
                CreateFee()
            };

            var model = MeNewLimitOrderModel.CreateMeStopLimitOrder(
                id,
                clientId,
                assetPairId,
                orderAction,
                volume,
                cancelAllPrevious,
                fee,
                fees,
                lowerLimitPrice,
                lowerPrice,
                upperLimitPrice,
                upperPrice
            );

            Assert.Equal(id, model.Id);
            Assert.Equal(clientId, model.ClientId);
            Assert.Equal(assetPairId, model.AssetPairId);
            Assert.Equal(-volume, model.Volume);
            Assert.Equal(cancelAllPrevious, model.CancelAllPreviousLimitOrders);
            Assert.Equal(fee, model.Fee);
            Assert.Equal(fees, model.Fees);
            Assert.Equal(1, model.Type);
            Assert.Equal(lowerLimitPrice, model.LowerLimitPrice);
            Assert.Equal(lowerPrice, model.LowerPrice);
            Assert.Equal(upperLimitPrice, model.UpperLimitPrice);
            Assert.Equal(upperPrice, model.UpperPrice);
        }
    }
}