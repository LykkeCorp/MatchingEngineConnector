using Lykke.MatchingEngine.Connector.Models.Common;
using Newtonsoft.Json;

namespace Lykke.MatchingEngine.Connector.Models.RabbitMq
{
    /// <summary>
    /// Client balance update.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/outgoing/messages/BalanceUpdate.kt"/>
    public class ClientBalanceUpdate : IValidatable
    {
        private static int MaxStringFieldsLength { get { return 255; } }

        /// <summary>Balance update client.</summary>
        [JsonProperty(PropertyName = "Id")]
        public string ClientId { get; set; }

        /// <summary>Balance update asset.</summary>
        public string Asset { get; set; }

        /// <summary>Old balance value.</summary>
        public double OldBalance { get; set; }

        /// <summary>New balance value.</summary>
        public double NewBalance { get; set; }

        /// <summary>Old reserved value.</summary>
        public double OldReserved { get; set; }

        /// <summary>New reserved value.</summary>
        public double NewReserved { get; set; }

        /// <inheritdoc />
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ClientId) && ClientId.Length <= MaxStringFieldsLength
                && !string.IsNullOrWhiteSpace(Asset) && Asset.Length <= MaxStringFieldsLength
                && (OldBalance != NewBalance || OldReserved != NewReserved);
        }
    }
}
