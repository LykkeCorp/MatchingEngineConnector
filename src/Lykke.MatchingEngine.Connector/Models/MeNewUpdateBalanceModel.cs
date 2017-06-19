using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class MeNewUpdateBalanceModel
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string ClientId { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public string AssetId { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public double Amount { get; set; }

        public static MeNewUpdateBalanceModel Create(string id, string clientId, string assetId, double amount)
        {
            return new MeNewUpdateBalanceModel
            {
                Id = id,
                ClientId = clientId,
                AssetId = assetId,
                Amount = amount
            };
        }
    }
}