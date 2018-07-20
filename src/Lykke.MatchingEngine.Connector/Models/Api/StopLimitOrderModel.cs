using System.Collections.Generic;
using Lykke.MatchingEngine.Connector.Models.Common;

namespace Lykke.MatchingEngine.Connector.Models.Api
{
    /// <summary>
    /// Stop limit order model.
    /// </summary>
    public class StopLimitOrderModel
    {
        /// <summary>
        /// Gets or sets the order/transaction identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the asset pair id.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// Gets or sets the order action (Buy or Sell).
        /// </summary>
        public OrderAction OrderAction { get; set; }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        public double Volume { get; set; }

        /// <summary>
        /// Cancel previous orders yes or no.
        /// </summary>
        public bool CancelPreviousOrders { get; set; }

        /// <summary>
        /// Gets or sets the fee.
        /// </summary>
        public LimitOrderFeeModel Fee { get; set; }

        /// <summary>
        /// Gets or sets the fees.
        /// </summary>
        public IReadOnlyList<LimitOrderFeeModel> Fees { get; set; } = new List<LimitOrderFeeModel>(0);

        /// <summary>
        /// Gets or sets the lower limit price.
        /// </summary>
        /// <remarks>
        /// When LowerLimitPrice is send then also LowerPrice is required y vice versa.
        /// </remarks>
        public double? LowerLimitPrice { get; set; }

        /// <summary>
        /// Gets or sets the lower price.
        /// </summary>
        /// <remarks>
        /// When LowerLimitPrice is send then also LowerPrice is required y vice versa.
        /// </remarks>
        public double? LowerPrice { get; set; }

        /// <summary>
        /// Gets or sets the upper limit price.
        /// </summary>
        /// <remarks>
        /// When UpperLimitPrice is send then also UpperPrice is required y vice versa.
        /// </remarks>
        public double? UpperLimitPrice { get; set; }

        /// <summary>
        /// Gets or sets the upper price.
        /// </summary>
        /// <remarks>
        /// When UpperLimitPrice is send then also UpperPrice is required y vice versa.
        /// </remarks>
        public double? UpperPrice { get; set; }
    }
}
