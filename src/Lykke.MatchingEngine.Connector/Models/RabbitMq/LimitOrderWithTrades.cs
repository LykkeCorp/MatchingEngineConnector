using Lykke.MatchingEngine.Connector.Models.Common;
using System.Collections.Generic;

namespace Lykke.MatchingEngine.Connector.Models.RabbitMq
{
    /// <summary>
    /// Container with limit order and its trades.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/outgoing/messages/LimitOrderWithTrades.kt"/>
    public class LimitOrderWithTrades : IValidatable
    {
        /// <summary>Limit order.</summary>
        public LimitOrder Order { get; set; }

        /// <summary>Limit order trades.</summary>
        public IReadOnlyList<LimitTradeInfo> Trades { get; set; }

        /// <inheritdoc />
        public bool IsValid()
        {
            if (!Order.IsValid())
                return false;

            if (Trades != null)
                foreach (var trade in Trades)
                {
                    if (!trade.IsValid())
                        return false;
                }

            return true;
        }
    }
}
