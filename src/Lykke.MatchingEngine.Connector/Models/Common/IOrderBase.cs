using System;

namespace Lykke.MatchingEngine.Connector.Models.Common
{
    /// <summary>
    /// Order base interface.
    /// </summary>
    public interface IOrderBase
    {
        /// <summary>Order id.</summary>
        string Id { get; set; }

        /// <summary>Order client.</summary>
        string ClientId { get; set; }

        /// <summary>Order creation timestamp.</summary>
        DateTime CreatedAt { get; set; }

        /// <summary>Order volume.</summary>
        double Volume { get; set; }

        /// <summary>Order price.</summary>
        double Price { get; set; }

        /// <summary>Order asset pair.</summary>
        string AssetPairId { get; set; }

        /// <summary>Order status.</summary>
        OrderStatus Status { get; set; }

        /// <summary>Order asset pair direction.</summary>
        bool Straight { get; set; }
    }
}