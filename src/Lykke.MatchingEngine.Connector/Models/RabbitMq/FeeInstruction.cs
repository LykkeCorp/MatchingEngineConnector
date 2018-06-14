using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Lykke.MatchingEngine.Connector.Models.Common;

namespace Lykke.MatchingEngine.Connector.Models.RabbitMq
{
    /// <summary>
    /// Fee instruction.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/daos/FeeInstruction.kt"/>
    public class FeeInstruction
    {
        /// <summary>Fee type.</summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public FeeType Type { get; set; }

        /// <summary>Fee size type.</summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public FeeSizeType SizeType { get; set; }

        /// <summary>Fee size.</summary>
        public double? Size { get; set; }

        /// <summary>Fee source client.</summary>
        public string SourceClientId { get; set; }

        /// <summary>Fee target client.</summary>
        public string TargetClientId { get; set; }
    }
}
