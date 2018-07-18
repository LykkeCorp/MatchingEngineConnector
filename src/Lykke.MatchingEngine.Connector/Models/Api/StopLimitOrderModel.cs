using System.Collections.Generic;
using Lykke.MatchingEngine.Connector.Models.Common;

namespace Lykke.MatchingEngine.Connector.Models.Api
{
    /// <summary>
    /// Stop limit order model.
    /// </summary>
    public class StopLimitOrderModel
    {
        public string Id { get; set; }

        public string ClientId { get; set; }

        public string AssetPairId { get; set; }

        public OrderAction OrderAction { get; set; }

        public double Volume { get; set; }

        public bool CancelPreviousOrders { get; set; }

        public LimitOrderFeeModel Fee { get; set; }

        public IReadOnlyList<LimitOrderFeeModel> Fees { get; set; } = new List<LimitOrderFeeModel>(0);

        public double LowerLimitPrice { get; set; }

        public double LowerPrice { get; set; }

        public double UpperLimitPrice { get; set; }

        public double UpperPrice { get; set; }
    }
}
