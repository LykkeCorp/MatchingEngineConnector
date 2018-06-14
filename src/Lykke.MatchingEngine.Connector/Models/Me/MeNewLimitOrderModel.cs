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

        [ProtoMember(6, IsRequired = true)]
        public double Price { get; set; }

        [ProtoMember(7, IsRequired = false)]
        public bool CancelAllPreviousLimitOrders { get; set; }

        [ProtoMember(8, IsRequired = false)]
        public MeLimitOrderFeeModel Fee { get; set; }
        
        [ProtoMember(9, IsRequired = false)]
        public MeLimitOrderFeeModel[] Fees { get; set; }

        public static MeNewLimitOrderModel Create(
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
            return new MeNewLimitOrderModel
            {
                Id = id,
                TimeStamp = (long) System.DateTime.UtcNow.ToUnixTime(),
                ClientId = clientId,
                AssetPairId = assetPairId,
                Volume = orderAction == OrderAction.Buy ? volume : -volume,
                Price = price,
                CancelAllPreviousLimitOrders = cancelAllPreviousLimitOrders,
                Fee = fee,
                Fees = fees
            };
        }
    }
}
