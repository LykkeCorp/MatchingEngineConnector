using Lykke.MatchingEngine.Connector.Models.Common;
using System;
using System.Collections.Generic;

namespace Lykke.MatchingEngine.Connector.Models.RabbitMq
{
    /// <summary>
    /// Market order trade info.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/outgoing/messages/TradeInfo.kt"/>
    public class TradeInfo : IValidatable
    {
        private static int MaxStringFieldsLength { get { return 255; } }

        /// <summary>Market order trade id.</summary>
        public string TradeId { get; set; }

        /// <summary>Market order trade client.</summary>
        public string MarketClientId { get; set; }

        /// <summary>Market order trade client.</summary>
        public double MarketVolume { get; set; }

        /// <summary>Market order trade asset.</summary>
        public string MarketAsset { get; set; }

        /// <summary>Market order trade price.</summary>
        public double Price { get; set; }

        /// <summary>Matched limit order trade client.</summary>
        public string LimitClientId { get; set; }

        /// <summary>Matched limit order trade volume.</summary>
        public double LimitVolume { get; set; }

        /// <summary>Matched limit order trade asset.</summary>
        public string LimitAsset { get; set; }

        /// <summary>Matched limit order ME id.</summary>
        public string LimitOrderId { get; set; }

        /// <summary>Matched limit order origin id.</summary>
        public string LimitOrderExternalId { get; set; }

        /// <summary>Market order trade timestamp.</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Market order trade fees.</summary>
        public IReadOnlyList<Fee> Fees { get; set; }

        /// <inheritdoc />
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(MarketClientId) && MarketClientId.Length <= MaxStringFieldsLength
                && MarketVolume != 0
                && !string.IsNullOrWhiteSpace(MarketAsset) && MarketAsset.Length <= MaxStringFieldsLength
                && !string.IsNullOrWhiteSpace(LimitClientId) && LimitClientId.Length <= MaxStringFieldsLength
                && LimitVolume != 0
                && !string.IsNullOrWhiteSpace(LimitAsset) && LimitAsset.Length <= MaxStringFieldsLength
                && !string.IsNullOrWhiteSpace(LimitOrderId) && LimitOrderId.Length <= MaxStringFieldsLength
                && !string.IsNullOrWhiteSpace(LimitOrderExternalId) && LimitOrderExternalId.Length <= MaxStringFieldsLength;
        }
    }
}
