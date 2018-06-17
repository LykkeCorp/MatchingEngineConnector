using Lykke.MatchingEngine.Connector.Models.Common;
using System;
using System.Collections.Generic;

namespace Lykke.MatchingEngine.Connector.Models.RabbitMq
{
    /// <summary>
    /// Balance update.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/outgoing/messages/BalanceUpdate.kt"/>
    public class BalanceUpdate : IValidatable
    {
        private static int MaxStringFieldsLength { get { return 255; } }

        /// <summary>Operation id.</summary>
        public string Id { get; set; }

        /// <summary>Operation type.</summary>
        public string Type { get; set; }

        /// <summary>Operation timestamp.</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Client balance updates for this operation.</summary>
        public IReadOnlyList<ClientBalanceUpdate> Balances { get; set; }

        /// <inheritdoc />
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Id) || Id.Length > MaxStringFieldsLength
                || string.IsNullOrWhiteSpace(Type) || Type.Length > MaxStringFieldsLength
                || Balances == null || Balances.Count == 0)
                return false;

            foreach (var balance in Balances)
            {
                if (!balance.IsValid())
                    return false;
            }

            return true;
        }
    }
}
