namespace Lykke.MatchingEngine.Connector.Models.Common
{
    /// <summary>
    /// Order status.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/order/OrderStatus.kt"/>
    public enum OrderStatus
    {
        /// <summary>Init status, limit order in order book</summary>
        InOrderBook,
        /// <summary>Partially matched</summary>
        Processing,
        /// <summary>Stop limit order pending</summary>
        Pending,
        /// <summary>Fully matched</summary>
        Matched,
        /// <summary>Not enough funds on account</summary>
        NotEnoughFunds,
        /// <summary>Reserved volume greater than balance</summary>
        ReservedVolumeGreaterThanBalance,
        /// <summary>No liquidity</summary>
        NoLiquidity,
        /// <summary>Unknown asset</summary>
        UnknownAsset,
        /// <summary>Disabled asset</summary>
        DisabledAsset,
        /// <summary>Cancelled</summary>
        Cancelled,
        /// <summary>Lead to negative spread</summary>
        LeadToNegativeSpread,
        /// <summary>Invalid fee</summary>
        InvalidFee,
        /// <summary>Too small volume</summary>
        TooSmallVolume,
        /// <summary>Invalid price</summary>
        InvalidPrice,
        /// <summary>Previous order is not found (by oldUid)</summary>
        NotFoundPrevious,
        /// <summary>Replaced</summary>
        Replaced,
        /// <summary>Invalid Price Accuracy</summary>
        InvalidPriceAccuracy,
        /// <summary>Invalid Volume Accuracy</summary>
        InvalidVolumeAccuracy,
    }
}
