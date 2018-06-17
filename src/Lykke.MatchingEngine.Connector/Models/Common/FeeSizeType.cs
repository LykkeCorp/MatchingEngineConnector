namespace Lykke.MatchingEngine.Connector.Models.Common
{
    /// <summary>
    /// Fee size type.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/daos/FeeSizeType.kt"/>
    public enum FeeSizeType
    {
        /// <summary>Percentage fee.</summary>
        PERCENTAGE = 0,
        /// <summary>Absolute fee.</summary>
        ABSOLUTE = 1,
    }
}
