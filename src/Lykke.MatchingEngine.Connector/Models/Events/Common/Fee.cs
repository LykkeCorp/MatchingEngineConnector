using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Events.Common
{
    [ProtoContract]
    public class Fee
    {
        [ProtoMember(1, IsRequired = true)]
        public FeeInstruction Instruction { get; set; }

        [ProtoMember(2, IsRequired = false)]
        public FeeTransfer Transfer { get; set; }
    }
}
