using Common;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class MeNewSwapModel
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string ClientId1 { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public string AssetId1 { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public double Amount1 { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public string ClientId2 { get; set; }

        [ProtoMember(6, IsRequired = true)]
        public string AssetId2 { get; set; }

        [ProtoMember(7, IsRequired = true)]
        public double Amount2 { get; set; }

        [ProtoMember(8, IsRequired = true)]
        public long DateTime { get; set; }

        public static MeNewSwapModel Create(string id,
            string clientId1, string assetId1, double amount1,
            string clientId2, string assetId2, double amount2)
        {
            return new MeNewSwapModel
            {
                Id = id,
                DateTime = (long)System.DateTime.UtcNow.ToUnixTime(),

                ClientId1 = clientId1,
                AssetId1 = assetId1,
                Amount1 = amount1,

                ClientId2 = clientId2,
                AssetId2 = assetId2,
                Amount2 = amount2
            };
        }
    }
}