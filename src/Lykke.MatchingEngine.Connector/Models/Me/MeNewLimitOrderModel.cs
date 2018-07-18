using Common;
using Lykke.MatchingEngine.Connector.Models.Common;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Me
{
    [ProtoContract]
    public class MeNewLimitOrderModel
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public long TimeStamp { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public string ClientId { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public string AssetPairId { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public double Volume { get; set; }

        [ProtoMember(6, IsRequired = false)]
        public double Price { get; set; }

        [ProtoMember(7, IsRequired = false)]
        public bool CancelAllPreviousLimitOrders { get; set; }

        [ProtoMember(8, IsRequired = false)]
        public MeLimitOrderFeeModel Fee { get; set; }

        [ProtoMember(9, IsRequired = false)]
        public MeLimitOrderFeeModel[] Fees { get; set; }

        [ProtoMember(10, IsRequired = false)]
        public int Type { get; set; }

        [ProtoMember(11, IsRequired = false)]
        public double LowerLimitPrice { get; set; }

        [ProtoMember(12, IsRequired = false)]
        public double LowerPrice { get; set; }

        [ProtoMember(13, IsRequired = false)]
        public double UpperLimitPrice { get; set; }

        [ProtoMember(14, IsRequired = false)]
        public double UpperPrice { get; set; }

        public static MeNewLimitOrderModel CreateMeLimitOrder(
            string id,
            string clientId,
            string assetPairId,
            OrderAction orderAction,
            double volume,
            double price,
            bool cancelAllPreviousLimitOrders,
            MeLimitOrderFeeModel fee,
            MeLimitOrderFeeModel[] fees)
        {
            var order = CreateBasicOrder(id, clientId, assetPairId, orderAction, volume, cancelAllPreviousLimitOrders,
                fee, fees, 0);
            order.Price = price;

            return order;
        }

        public static MeNewLimitOrderModel CreateMeStopLimitOrder(
            string id,
            string clientId,
            string assetPairId,
            OrderAction orderAction,
            double volume,
            bool cancelAllPreviousLimitOrders,
            MeLimitOrderFeeModel fee,
            MeLimitOrderFeeModel[] fees,
            double lowerLimitPrice,
            double lowerPrice,
            double upperLimitPrice,
            double upperPrice)
        {
            var order = CreateBasicOrder(id, clientId, assetPairId, orderAction, volume, cancelAllPreviousLimitOrders,
                fee, fees, 1);

            order.LowerLimitPrice = lowerLimitPrice;
            order.LowerPrice = lowerPrice;
            order.UpperLimitPrice = upperLimitPrice;
            order.UpperPrice = upperPrice;

            return order;
        }

        private static MeNewLimitOrderModel CreateBasicOrder(
            string id,
            string clientId,
            string assetPairId,
            OrderAction orderAction,
            double volume,
            bool cancelAllPreviousLimitOrders,
            MeLimitOrderFeeModel fee,
            MeLimitOrderFeeModel[] fees,
            int type)
        {
            return new MeNewLimitOrderModel
            {
                Id = id,
                TimeStamp = (long)System.DateTime.UtcNow.ToUnixTime(),
                ClientId = clientId,
                AssetPairId = assetPairId,
                Volume = orderAction == OrderAction.Buy ? volume : -volume,
                CancelAllPreviousLimitOrders = cancelAllPreviousLimitOrders,
                Fee = fee,
                Fees = fees,
                Type = type
            };
        }
    }
}
