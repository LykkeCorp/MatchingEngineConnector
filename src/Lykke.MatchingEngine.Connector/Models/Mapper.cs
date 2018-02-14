using System.Linq;
using Lykke.MatchingEngine.Connector.Abstractions.Models;

namespace Lykke.MatchingEngine.Connector.Models
{
    public static class Mapper
    {
        public static MeMarketOrderModel ToMeModel(this MarketOrderModel model)
        {
            return MeMarketOrderModel.Create(model.Id, model.ClientId, model.AssetPairId, model.OrderAction,
                model.Volume, model.Straight, model.ReservedLimitVolume, model.Fee?.ToMeModel(), model.Fees.Select(item => item.ToMeModel()).ToArray());
        }

        public static MeMarketOrderFeeModel ToMeModel(this MarketOrderFeeModel model)
        {
            return MeMarketOrderFeeModel.Create(model.Type, model.Size, model.SourceClientId, model.TargetClientId, model.SizeType, model.AssetId);
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

        public static MeMultiLimitOrderModel ToMeModel(this MultiLimitOrderModel model)
        {
            return MeMultiLimitOrderModel.Create(
                model.Id, model.ClientId, model.AssetId,
                model.Orders.Select(m => MeMultiOrderItemModel.Create(
                    m.Id, m.OrderAction, m.Volume, m.Price, m.Fee?.ToMeModel())).ToArray(),
                model.CancelPreviousOrders);
        }

        public static MeMultiLimitOrderCancelModel ToMeModel(this MultiLimitOrderCancelModel model)
        {
            return MeMultiLimitOrderCancelModel.Create(model.Id, model.ClientId, model.AssetPairId, model.IsBuy);
        }
    }
}
