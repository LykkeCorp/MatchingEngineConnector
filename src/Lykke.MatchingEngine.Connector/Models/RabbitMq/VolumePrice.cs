using Lykke.MatchingEngine.Connector.Models.Common;

namespace Lykke.MatchingEngine.Connector.Models.RabbitMq
{
    /// <summary>
    /// Order data.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/outgoing/messages/OrderBook.kt"/>
    public class VolumePrice : IValidatable
    {
        /// <summary>Order volume.</summary>
        public double Volume { get; set; }

        /// <summary>Order price.</summary>
        public double Price { get; set; }

        /// <inheritdoc />
        public bool IsValid()
        {
            return Volume != 0 && Price >= 0;
        }
    }
}
