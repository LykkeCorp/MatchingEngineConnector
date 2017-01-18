using Common;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class MeCashInOutModel
    {
        [ProtoMember(1, IsRequired = true)]
        public long Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string ClientId { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public long DateTime { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public string AssetId { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public double Amount { get; set; }

        [ProtoMember(6, IsRequired = true)]
        public string CorrelationId { get; set; }

        [ProtoMember(7, IsRequired = true)]
        public bool SendToBitcoin { get; set; }

        public static MeCashInOutModel Create(long id, string clientId, string assetId, double amount, bool sendToBitcoin, string correlationId)
        {
            return new MeCashInOutModel
            {
                Id = id,
                DateTime = (long)System.DateTime.UtcNow.ToUnixTime(),
                ClientId = clientId,
                AssetId = assetId,
                Amount = amount,
                CorrelationId = correlationId,
                SendToBitcoin = sendToBitcoin
            };
        }
    }
}