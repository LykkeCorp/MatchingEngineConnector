using System.Collections.Generic;
using Lykke.MatchingEngine.Connector.Models.Events.Common;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Events
{
    [ProtoContract]
    public class ExecutionEvent
    {
        [ProtoMember(1, IsRequired = true)]
        public Header Header { get; set; }

        [ProtoMember(2, IsRequired = false)]
        public List<BalanceUpdate> BalanceUpdates { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public List<Order> Orders { get; set; }
    }
}
