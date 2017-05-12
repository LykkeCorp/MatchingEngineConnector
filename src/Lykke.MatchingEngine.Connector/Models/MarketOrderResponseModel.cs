using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class MarketOrderResponseModel
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public int Status { get; set; }

        [ProtoMember(3, IsRequired = false)]
        public string StatusReason { get; set; }

        [ProtoMember(4, IsRequired = false)]
        public double Price { get; set; }
    }
}
