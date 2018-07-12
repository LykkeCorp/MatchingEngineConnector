using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Events.Common
{
    [ProtoContract]
    public class FeeTransfer
    {
        [ProtoMember(1, IsRequired = true)]
        public string Volume { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string SourceWalletId { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public string TargetWalletId { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public string AssetId { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public string FeeCoef { get; set; }

        /// <summary>
        /// Field to link instruction from order to transfer on trade
        /// </summary>
        [ProtoMember(6, IsRequired = true)]
        public int Index { get; set; }
    }
}
