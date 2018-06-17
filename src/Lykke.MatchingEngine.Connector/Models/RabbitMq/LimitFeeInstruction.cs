using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Lykke.MatchingEngine.Connector.Models.Common;

namespace Lykke.MatchingEngine.Connector.Models.RabbitMq
{
    /// <summary>
    /// Fee instruction for limit order trades.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/daos/fee/NewLimitOrderFeeInstruction.kt"/>
    public class LimitFeeInstruction
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

        /// <summary>Maker fee size type.</summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public FeeSizeType? MakerSizeType { get; set; }

        /// <summary>Maker fee size.</summary>
        public double? MakerSize { get; set; }

        /// <summary>Maker fee modificator.</summary>
        public double? MakerFeeModificator { get; set; }

        /// <summary>Taker fee size type.</summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public FeeSizeType? TakerSizeType { get; set; }

        /// <summary>Taker fee size.</summary>
        public double? TakerSize { get; set; }
    }
}
