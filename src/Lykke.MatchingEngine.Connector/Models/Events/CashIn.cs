using System.Collections.Generic;
using Lykke.MatchingEngine.Connector.Models.Events.Common;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Events
{
    [ProtoContract]
    public class CashIn
    {
        [ProtoMember(1, IsRequired = true)]
        public string WalletId { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string AssetId { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public string Volume { get; set; }

        [ProtoMember(4, IsRequired = false)]
        public List<Fee> Fees { get; set; }
    }
}
