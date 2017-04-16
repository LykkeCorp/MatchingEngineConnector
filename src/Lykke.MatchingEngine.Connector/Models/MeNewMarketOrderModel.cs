using Common;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class MeNewMarketOrderModel
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public long DateTime { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public string ClientId { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public string AssetPairId { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public double Volume { get; set; }

        [ProtoMember(6, IsRequired = true)]
        public bool Straight { get; set; }

        [ProtoMember(7, IsRequired = false)]
        public double? ReservedLimitVolume { get; set; }

        public static MeNewMarketOrderModel Create(string id, string clientId, string assetPairId, OrderAction orderAction, double volume, bool straight, double? reservedLimitVolume)
        {
            return new MeNewMarketOrderModel
            {
                Id = id,
                DateTime = (long)System.DateTime.UtcNow.ToUnixTime(),
                ClientId = clientId,
                AssetPairId = assetPairId,
                Volume = orderAction == OrderAction.Buy ? volume : -volume,
                Straight = straight,
                ReservedLimitVolume = reservedLimitVolume
            };
        }
    }
}