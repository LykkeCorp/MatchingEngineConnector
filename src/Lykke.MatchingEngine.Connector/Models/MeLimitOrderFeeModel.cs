using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class MeLimitOrderFeeModel
    {
        [ProtoMember(1, IsRequired = true)]
        public int Type { get; set; }
        [ProtoMember(2, IsRequired = false)]
        public double MakerSize { get; set; }
        [ProtoMember(3, IsRequired = false)]
        public double TakerSize { get; set; }
        [ProtoMember(4, IsRequired = false)]
        public string SourceClientId { get; set; }
        [ProtoMember(5, IsRequired = false)]
        public string TargetClientId { get; set; }

        public static MeLimitOrderFeeModel Create(int type, double makerSize, double takerSize, string sourceClientId,
            string targetClientId)
        {
            return new MeLimitOrderFeeModel
            {
                Type = type,
                MakerSize = makerSize,
                TakerSize = takerSize,
                SourceClientId = sourceClientId,
                TargetClientId = targetClientId
            };
        }
    }
}
