using Lykke.MatchingEngine.Connector.Models.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Lykke.MatchingEngine.Connector.Models.RabbitMq
{
    /// <summary>
    /// Market order.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/daos/MarketOrder.kt"/>
    public class MarketOrder : IValidatable
    {
        private static int MaxStringFieldsLength { get { return 255; } }

        /// <summary>Order id in ME.</summary>
        public string Id { get; set; }

        /// <summary>Order origin id.</summary>
        public string ExternalId { get; set; }

        /// <summary>Order asset pair.</summary>
        public string AssetPairId { get; set; }

        /// <summary>Order client.</summary>
        public string ClientId { get; set; }

        /// <summary>Order volume.</summary>
        public double Volume { get; set; }

        /// <summary>Order price.</summary>
        public double? Price { get; set; }

        /// <summary>Order status.</summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderStatus Status { get; set; }

        /// <summary>Order last status change timestamp.</summary>
        public DateTime? StatusDate { get; set; }

        /// <summary>Order creation timestamp.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Order registration timestamp.</summary>
        public DateTime Registered { get; set; }

        /// <summary>Order last match timestamp.</summary>
        public DateTime? MatchedAt { get; set; }

        /// <summary>Order asset pair direction.</summary>
        public bool Straight { get; set; }

        /// <summary>Order reserved limit volume.</summary>
        public double? ReservedLimitVolume { get; set; }

        /// <summary>Dust size.</summary>
        public double? DustSize { get; set; }

        /// <inheritdoc />
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Id) && Id.Length <= MaxStringFieldsLength
                && !string.IsNullOrWhiteSpace(ExternalId) && ExternalId.Length <= MaxStringFieldsLength
                && !string.IsNullOrWhiteSpace(AssetPairId) && AssetPairId.Length <= MaxStringFieldsLength
                && !string.IsNullOrWhiteSpace(ClientId) && ClientId.Length <= MaxStringFieldsLength
                && (!Price.HasValue || Price.Value != 0)
                && Volume != 0;
        }
    }
}
