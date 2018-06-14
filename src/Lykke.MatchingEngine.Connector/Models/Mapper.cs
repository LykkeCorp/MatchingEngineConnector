using Lykke.MatchingEngine.Connector.Models.Api;
using Lykke.MatchingEngine.Connector.Models.Me;
using System.Linq;

namespace Lykke.MatchingEngine.Connector.Models
{
    public static class Mapper
    {
        public static MeMarketOrderModel ToMeModel(this MarketOrderModel model)
        {
            return MeMarketOrderModel.Create(
                model.Id,
                model.ClientId,
                model.AssetPairId,
                model.OrderAction,
                model.Volume,
                model.Straight,
                model.ReservedLimitVolume,
                model.Fee?.ToMeModel(),
                model.Fees.Select(item => item.ToMeModel()).ToArray());
        }

        public static MeMarketOrderFeeModel ToMeModel(this MarketOrderFeeModel model)
        {
            return MeMarketOrderFeeModel.Create(
                model.Type,
                model.Size,
                model.SourceClientId,
                model.TargetClientId,
                model.SizeType,
                model.AssetId.ToArray());
        }

        public static MeNewLimitOrderModel ToNewMeModel(this LimitOrderModel model)
        {
            return MeNewLimitOrderModel.Create(
                model.Id,
                model.ClientId,
                model.AssetPairId,
                model.OrderAction,
                model.Volume,
                model.Price,
                model.CancelPreviousOrders,
                model.Fee?.ToMeModel(),
                model.Fees?.Select(item => item.ToMeModel()).ToArray());
        }

        public static MeLimitOrderFeeModel ToMeModel(this LimitOrderFeeModel model)
        {
            return MeLimitOrderFeeModel.Create(
                model.Type,
                model.MakerSize,
                model.TakerSize,
                model.SourceClientId,
                model.TargetClientId,
                model.MakerSizeType,
                model.TakerSizeType,
                model.AssetId.ToArray(),
                model.MakerFeeModificator);
        }

        public static MeMultiLimitOrderModel ToMeModel(this MultiLimitOrderModel model)
        {
            return MeMultiLimitOrderModel.Create(
                model.Id,
                model.ClientId,
                model.AssetId,
                model.Orders
                    .Select(m => MeMultiOrderItemModel.Create(m.Id, m.OrderAction, m.Volume, m.Price, m.Fee?.ToMeModel(), m.OldId))
                    .ToArray(),
                model.CancelPreviousOrders,
                model.CancelMode);
        }

        public static MeMultiLimitOrderCancelModel ToMeModel(this MultiLimitOrderCancelModel model)
        {
            return MeMultiLimitOrderCancelModel.Create(
                model.Id,
                model.ClientId,
                model.AssetPairId,
                model.IsBuy);
        }

        public static MeLimitOrderMassCancelModel ToMeModel(this LimitOrderMassCancelModel model)
        {
            return MeLimitOrderMassCancelModel.Create(
                model.Id,
                model.ClientId,
                model.AssetPairId,
                model.IsBuy);
        }
    }
}
