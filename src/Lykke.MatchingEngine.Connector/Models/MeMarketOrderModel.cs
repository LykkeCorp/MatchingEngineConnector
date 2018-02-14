using Common;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class MeMarketOrderModel
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public long DateTime { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public string ClientId { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public string AssetId { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public double Volume { get; set; }

        [ProtoMember(6, IsRequired = true)]
        public bool Straight { get; set; }

        [ProtoMember(7, IsRequired = false)]
        public double? ReservedLimitVolume { get; set; }

        [ProtoMember(8, IsRequired = false)]
        public MeMarketOrderFeeModel Fee { get; set; }
        
        [ProtoMember(9, IsRequired = false)]
        public MeMarketOrderFeeModel[] Fees { get; set; }

        public static MeMarketOrderModel Create(string id, string clientId, string assetId, OrderAction orderAction,
            double volume, bool straight, double? reservedLimitVolume, MeMarketOrderFeeModel fee, MeMarketOrderFeeModel[] fees)
        {
            return new MeMarketOrderModel
            {
                Id = id,
                DateTime = (long) System.DateTime.UtcNow.ToUnixTime(),
                ClientId = clientId,
                AssetId = assetId,
                Volume = orderAction == OrderAction.Buy ? volume : -volume,
                Straight = straight,
                ReservedLimitVolume = reservedLimitVolume,
                Fee = fee,
                Fees = fees
            };
        }
    }
}
