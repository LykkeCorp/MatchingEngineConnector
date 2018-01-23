using Common;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class MeNewCashInOutModel
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string ClientId { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public long DateTime { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public string AssetId { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public double Amount { get; set; }

        [ProtoMember(6, IsRequired = false)]
        public Fee Fee { get; set; }
        
        public static MeNewCashInOutModel Create(string id, string clientId,
            string assetId, double amount, Fee fee = null)
        {
            return new MeNewCashInOutModel
            {
                Id = id,
                ClientId = clientId,
                DateTime = (long)System.DateTime.UtcNow.ToUnixTime(),
                AssetId = assetId,
                Amount = amount,
                Fee = fee
            };
        }
    }

    public static class FeeExtensions
    {
        public static FeeType GetFeeType(this double feeSize)
        {
            return feeSize > 0.0 ? FeeType.CLIENT_FEE : FeeType.NO_FEE;
        }
    }
}
