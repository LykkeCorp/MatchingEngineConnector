namespace Lykke.MatchingEngine.Connector.Models.Common
{
    /// <summary>
    /// Fee type enum.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/daos/FeeType.kt"/>
    public enum FeeType
    {
        /// <summary>No fee.</summary>
        NO_FEE = 0,
        /// <summary>Client fee.</summary>
        CLIENT_FEE = 1,
        /// <summary>External fee.</summary>
        EXTERNAL_FEE = 2,
    }
}
