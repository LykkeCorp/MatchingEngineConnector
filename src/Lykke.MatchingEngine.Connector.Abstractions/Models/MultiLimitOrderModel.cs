namespace Lykke.MatchingEngine.Connector.Abstractions.Models
{
    public class MultiLimitOrderModel
    {
        public string Id { get; set; } 
        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public bool CancelPreviousOrders { get; set; }
        public MultiOrderItemModel[] Orders { get; set; }
    }
}