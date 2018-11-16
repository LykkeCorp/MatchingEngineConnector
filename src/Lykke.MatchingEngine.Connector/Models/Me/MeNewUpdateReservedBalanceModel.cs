using Common;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Me
{
    [ProtoContract]
    public class MeNewUpdateReservedBalanceModel
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string ClientId { get; set; }
        
        [ProtoMember(3, IsRequired = true)]
        public long TimeStamp { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public string AssetId { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public double ReservedVolume { get; set; }

        public static MeNewUpdateReservedBalanceModel Create(
            string id,
            string clientId,
            string assetId,
            double amount)
        {
            return new MeNewUpdateReservedBalanceModel
            {
                Id = id,
                TimeStamp = (long)System.DateTime.UtcNow.ToUnixTime(),
                ClientId = clientId,
                AssetId = assetId,
                ReservedVolume = amount
            };
        }
    }
}