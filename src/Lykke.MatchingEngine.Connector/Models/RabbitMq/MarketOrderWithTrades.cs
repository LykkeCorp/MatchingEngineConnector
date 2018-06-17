using Lykke.MatchingEngine.Connector.Models.Common;
using System.Collections.Generic;

namespace Lykke.MatchingEngine.Connector.Models.RabbitMq
{
    /// <summary>
    /// Container with market order and its trades.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/outgoing/messages/MarketOrderWithTrades.kt"/>
    public class MarketOrderWithTrades : IValidatable
    {
        /// <summary>Market order.</summary>
        public MarketOrder Order { get; set; }

        /// <summary>Market order trades.</summary>
        public IReadOnlyList<TradeInfo> Trades { get; set; }

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
