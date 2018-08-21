using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Events.Common
{
    [ProtoContract]
    public class BalanceUpdate
    {
        [ProtoMember(1, IsRequired = true)]
        public string WalletId { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string AssetId { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public string OldBalance { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public string NewBalance { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public string OldReserved { get; set; }

        [ProtoMember(6, IsRequired = true)]
        public string NewReserved { get; set; }
    }
}
