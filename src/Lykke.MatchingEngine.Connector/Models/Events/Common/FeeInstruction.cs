using System.Collections.Generic;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Events.Common
{
    [ProtoContract]
    public class FeeInstruction
    {
        [ProtoMember(1, IsRequired = true)]
        public FeeInstructionType Type { get; set; }

        [ProtoMember(2, IsRequired = false)]
        public string Size { get; set; }

        [ProtoMember(3, IsRequired = false)]
        public FeeInstructionSizeType SizeType { get; set; }

        [ProtoMember(4, IsRequired = false)]
        public string MakerSize { get; set; }

        [ProtoMember(5, IsRequired = false)]
        public FeeInstructionSizeType MakerSizeType { get; set; }

        [ProtoMember(6, IsRequired = false)]
        public string SourceWalletd { get; set; }

        [ProtoMember(7, IsRequired = false)]
        public string TargetWalletId { get; set; }

        [ProtoMember(8, IsRequired = false)]
        public List<string> AssetsIds { get; set; }

        [ProtoMember(9, IsRequired = false)]
        public string MakerFeeModificator { get; set; }

        /// <summary>
        /// Field to link instruction from order to transfer on trade
        /// </summary>
        [ProtoMember(10, IsRequired = true)]
        public int Index { get; set; }
    }
}
