using System.Collections.Generic;
using Lykke.MatchingEngine.Connector.Models.Events.Common;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Events
{
    [ProtoContract]
    public class CashInEvent
    {
        [ProtoMember(1, IsRequired = true)]
        public Header Header { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public List<BalanceUpdate> BalanceUpdates { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public CashIn CashIn { get; set; }
    }
}
