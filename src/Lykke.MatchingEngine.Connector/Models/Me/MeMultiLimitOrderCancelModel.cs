using Common;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Me
{
    [ProtoContract]
    public class MeMultiLimitOrderCancelModel
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
        public bool IsBuy { get; set; }

        public static MeMultiLimitOrderCancelModel Create(
            string id,
            string clientId,
            string assetId,
            bool isBuy)
        {
            return new MeMultiLimitOrderCancelModel
            {
                Id = id,
                DateTime = (long)System.DateTime.UtcNow.ToUnixTime(),
                ClientId = clientId,
                AssetId = assetId,
                IsBuy = isBuy
            };
        }
    }
}