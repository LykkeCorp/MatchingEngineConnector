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
        [ProtoMember(6, IsRequired = false)]
        public int MakerSizeType { get; set; }
        [ProtoMember(7, IsRequired = false)]
        public int TakerSizeType { get; set; }
        [ProtoMember(8, IsRequired = false)]
        public string[] AssetId { get; set; }
        [ProtoMember(9, IsRequired = false)]
        public double MakerFeeModificator { get; set; }

        public static MeLimitOrderFeeModel Create(int type, double makerSize, double takerSize, string sourceClientId,
            string targetClientId, int makerSizeType, int takerSizeType, string[] assetId, double makerFeeModificator)
        {
            return new MeLimitOrderFeeModel
            {
                Type = type,
                MakerSize = makerSize,
                TakerSize = takerSize,
                SourceClientId = sourceClientId,
                TargetClientId = targetClientId,
                MakerSizeType = makerSizeType,
                TakerSizeType = takerSizeType,
                AssetId = assetId,
                MakerFeeModificator = makerFeeModificator
            };
        }
    }
}
