using System.Collections.Generic;

namespace Lykke.MatchingEngine.Connector.Abstractions.Models
{
    public class MultiLimitOrderModel
    {
        public string Id { get; set; } 
        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public bool CancelPreviousOrders { get; set; }
        public CancelMode CancelMode { get; set; }
        public IReadOnlyList<MultiOrderItemModel> Orders { get; set; }
    }
}