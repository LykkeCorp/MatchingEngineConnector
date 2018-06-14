namespace Lykke.MatchingEngine.Connector.Models.Common
{
    /// <summary>
    /// Fee charging type
    /// </summary>
    public enum FeeChargingType
    {
        /// <summary>Unknown charge.</summary>
        UNKNOWN = 0,
        /// <summary>Subtract from amount charge.</summary>
        SUBTRACT_FROM_AMOUNT = 1,
        /// <summary>Rise amount charge.</summary>
        RAISE_AMOUNT = 2,
    }
}
