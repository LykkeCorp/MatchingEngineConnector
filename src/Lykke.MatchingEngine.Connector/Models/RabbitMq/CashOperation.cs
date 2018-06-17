using Lykke.MatchingEngine.Connector.Models.Common;
using System;
using System.Collections.Generic;

namespace Lykke.MatchingEngine.Connector.Models.RabbitMq
{
    /// <summary>
    /// Cash in/out operation.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/outgoing/messages/CashOperation.kt"/>
    public class CashOperation : IValidatable
    {
        private static int MaxStringFieldsLength { get { return 255; } }

        /// <summary>Operation id.</summary>
        public string Id { get; set; }

        /// <summary>Operation client.</summary>
        public string ClientId { get; set; }

        /// <summary>Operation volume.</summary>
        public double Volume { get; set; }

        /// <summary>Operation asset.</summary>
        public string Asset { get; set; }

        /// <summary>Operation timestamp.</summary>
        public DateTime DateTime { get; set; }

        /// <summary>Operation fees.</summary>
        public IReadOnlyList<Fee> Fees { get; set; }

        /// <inheritdoc />
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Id) && Id.Length <= MaxStringFieldsLength
                && !string.IsNullOrWhiteSpace(ClientId) && ClientId.Length <= MaxStringFieldsLength
                && !string.IsNullOrWhiteSpace(Asset) && Asset.Length <= MaxStringFieldsLength
                && Volume != 0;
        }
    }
}
