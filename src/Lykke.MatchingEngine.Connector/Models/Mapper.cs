using Lykke.MatchingEngine.Connector.Abstractions.Models;

namespace Lykke.MatchingEngine.Connector.Models
{
    public static class Mapper
    {
        public static MeMarketOrderModel ToMeModel(this MarketOrderModel model)
        {
            return MeMarketOrderModel.Create(model.Id, model.ClientId, model.AssetPairId, model.OrderAction,
                model.Volume, model.Straight, model.ReservedLimitVolume, model.Fee?.ToMeModel());
        }

        public static MeMarketOrderFeeModel ToMeModel(this MarketOrderFeeModel model)
        {
            return MeMarketOrderFeeModel.Create(model.Type, model.Size, model.SourceClientId, model.TargetClientId);
        }

        public static MeNewLimitOrderModel ToNewMeModel(this LimitOrderModel model)
        {
            return MeNewLimitOrderModel.Create(model.Id, model.ClientId, model.AssetPairId, model.OrderAction,
                model.Volume, model.Price, model.CancelPreviousOrders, model.Fee?.ToMeModel());
        }

        public static MeLimitOrderFeeModel ToMeModel(this LimitOrderFeeModel model)
        {
            return MeLimitOrderFeeModel.Create(model.Type, model.MakerSize, model.TakerSize, model.SourceClientId,
                model.TargetClientId);
        }
    }
}
