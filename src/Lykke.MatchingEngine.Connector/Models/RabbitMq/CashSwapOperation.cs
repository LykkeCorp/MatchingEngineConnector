using Lykke.MatchingEngine.Connector.Models.Common;
using System;

namespace Lykke.MatchingEngine.Connector.Models.RabbitMq
{
    /// <summary>
    /// Cash operation.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/outgoing/messages/CashSwapOperation.kt"/>
    public class CashSwapOperation : IValidatable
    {
        private static int MaxStringFieldsLength { get { return 255; } }

        /// <summary>Operation id.</summary>
        public string Id { get; set; }

        /// <summary>Operation timestamp.</summary>
        public DateTime DateTime { get; set; }

        /// <summary>First operation client.</summary>
        public string ClientId1 { get; set; }

        /// <summary>First operation volume.</summary>
        public double Volume1 { get; set; }

        /// <summary>First operation asset.</summary>
        public string Asset1 { get; set; }

        /// <summary>Second operation client.</summary>
        public string ClientId2 { get; set; }

        /// <summary>Second operation volume.</summary>
        public double Volume2 { get; set; }

        /// <summary>Second operation asset.</summary>
        public string Asset2 { get; set; }

        /// <inheritdoc />
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Id) && Id.Length <= MaxStringFieldsLength
                && !string.IsNullOrWhiteSpace(ClientId1) && ClientId1.Length <= MaxStringFieldsLength
                && !string.IsNullOrWhiteSpace(Asset1) && Asset1.Length <= MaxStringFieldsLength
                && Volume1 != 0
                && !string.IsNullOrWhiteSpace(ClientId2) && ClientId2.Length <= MaxStringFieldsLength
                && !string.IsNullOrWhiteSpace(Asset2) && Asset2.Length <= MaxStringFieldsLength
                && Volume2 != 0
                && ClientId1 != ClientId2;
        }
    }
}
