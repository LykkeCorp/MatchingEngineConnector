using Lykke.MatchingEngine.Connector.Models.Common;
using System;
using System.Collections.Generic;

namespace Lykke.MatchingEngine.Connector.Models.RabbitMq
{
    /// <summary>
    /// Limit order trade info.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/outgoing/messages/LimitTradeInfo.kt"/>
    public class LimitTradeInfo : IValidatable
    {
        private static int MaxStringFieldsLength { get { return 255; } }

        /// <summary>Limit order trade id.</summary>
        public string TradeId { get; set; }

        /// <summary>Limit order trade client.</summary>
        public string ClientId { get; set; }

        /// <summary>Limit order trade asset.</summary>
        public string Asset { get; set; }

        /// <summary>Limit order trade volume.</summary>
        public double Volume { get; set; }

        /// <summary>Limit order trade price.</summary>
        public double Price { get; set; }

        /// <summary>Limit order trade timestamp.</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Limit order trade opposite order ME id.</summary>
        public string OppositeOrderId { get; set; }

        /// <summary>Limit order trade opposite order origin id.</summary>
        public string OppositeOrderExternalId { get; set; }

        /// <summary>Limit order trade opposite asset.</summary>
        public string OppositeAsset { get; set; }

        /// <summary>Limit order trade opposite client.</summary>
        public string OppositeClientId { get; set; }

        /// <summary>Limit order trade opposite volume.</summary>
        public double OppositeVolume { get; set; }

        /// <summary>Limit order trade fees.</summary>
        public IReadOnlyList<LimitFee> Fees { get; set; }

        /// <inheritdoc />
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ClientId) && ClientId.Length <= MaxStringFieldsLength
                && !string.IsNullOrWhiteSpace(Asset) && Asset.Length <= MaxStringFieldsLength
                && Volume != 0
                && Price > 0
                && !string.IsNullOrWhiteSpace(OppositeClientId) && OppositeClientId.Length <= MaxStringFieldsLength
                && OppositeVolume != 0
                && !string.IsNullOrWhiteSpace(OppositeAsset) && OppositeAsset.Length <= MaxStringFieldsLength
                && !string.IsNullOrWhiteSpace(OppositeOrderId) && OppositeOrderId.Length <= MaxStringFieldsLength
                && !string.IsNullOrWhiteSpace(OppositeOrderExternalId) && OppositeOrderExternalId.Length <= MaxStringFieldsLength;
        }
    }
}
