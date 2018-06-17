using System;

namespace Lykke.MatchingEngine.Connector.Models.RabbitMq
{
    /// <summary>
    /// Fee transfer.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/daos/FeeTransfer.kt"/>
    public class FeeTransfer
    {
        /// <summary>External fee id.</summary>
        public string ExternalId { get; set; }

        /// <summary>Fee source client.</summary>
        public string FromClientId { get; set; }

        /// <summary>Fee target client.</summary>
        public string ToClientId { get; set; }

        /// <summary>Fee transfer timestamp.</summary>
        public DateTime DateTime { get; set; }

        /// <summary>Fee transfer volume.</summary>
        public double Volume { get; set; }

        /// <summary>Fee transfer asset.</summary>
        public string Asset { get; set; }

        /// <summary>Fee transfer coefficient.</summary>
        public double? FeeCoef { get; set; }
    }
}
