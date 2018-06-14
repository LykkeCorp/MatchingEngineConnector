using Lykke.MatchingEngine.Connector.Models.Common;
using System.Linq;
using System.Collections.Generic;

namespace Lykke.MatchingEngine.Connector.Models.RabbitMq
{
    /// <summary>
    /// Limit orders container.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/outgoing/messages/LimitOrdersReport.kt"/>
    public class LimitOrders : IValidatable
    {
        /// <summary>Limit orders.</summary>
        public IReadOnlyList<LimitOrderWithTrades> Orders { get; set; }

        /// <inheritdoc />
        public bool IsValid()
        {
            return Orders.All(o => o.IsValid());
        }
    }
}
