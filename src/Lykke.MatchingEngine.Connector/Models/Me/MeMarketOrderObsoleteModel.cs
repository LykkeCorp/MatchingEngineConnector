using Common;
using Lykke.MatchingEngine.Connector.Models.Common;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Me
{
    [ProtoContract]
    public class MeMarketOrderObsoleteModel
    {
        [ProtoMember(1, IsRequired = true)]
        public long Id { get; set; }

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

        public static MeMarketOrderObsoleteModel Create(
            long id,
            string clientId,
            string assetId,
            OrderAction orderAction,
            double volume,
            bool straight,
            double? reservedLimitVolume)
        {
            return new MeMarketOrderObsoleteModel
            {
                Id = id,
                DateTime = (long)System.DateTime.UtcNow.ToUnixTime(),
                ClientId = clientId,
                AssetId = assetId,
                Volume = orderAction == OrderAction.Buy ? volume : -volume,
                Straight = straight,
                ReservedLimitVolume = reservedLimitVolume
            };
        }
    }
}