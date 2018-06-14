using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Me
{
    [ProtoContract]
    public class MeLimitOrderMassCancelModel
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string ClientId { get; set; }

        [ProtoMember(3, IsRequired = false)]
        public string AssetPairId { get; set; }

        [ProtoMember(4, IsRequired = false)]
        public bool? IsBuy { get; set; }

        public static MeLimitOrderMassCancelModel Create(
            string id,
            string clientId,
            string assetPairId,
            bool? isBuy)
        {
            return new MeLimitOrderMassCancelModel
            {
                Id = id,
                ClientId = clientId,
                AssetPairId = assetPairId,
                IsBuy = isBuy
            };
        }
    }
}