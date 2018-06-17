using Lykke.MatchingEngine.Connector.Models.Common;
using System;
using System.Collections.Generic;

namespace Lykke.MatchingEngine.Connector.Models.RabbitMq
{
    /// <summary>
    /// Order book.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/outgoing/messages/OrderBook.kt"/>
    public class OrderBook : IValidatable
    {
        private static int MaxStringFieldsLength { get { return 255; } }

        /// <summary>Orderbook asset pair.</summary>
        public string AssetPair { get; set; }

        /// <summary>Is buy flag.</summary>
        public bool IsBuy { get; set; }

        /// <summary>Orderbook timestamp.</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Orderbook volume prices.</summary>
        public IReadOnlyList<VolumePrice> Prices { get; set; }

        /// <inheritdoc />
        public bool IsValid()
        {
            if (AssetPair == null || AssetPair.Length > MaxStringFieldsLength)
                return false;

            if (Prices != null)
                foreach (var price in Prices)
                {
                    if (!price.IsValid())
                        return false;
                }

            return true;
        }
    }
}
