namespace Lykke.MatchingEngine.Connector.Abstractions.Models
{
    public class MultiOrderItemModel
    {
        public string Id { get; set; }
        public OrderAction OrderAction { get; set; }
        public double Volume { get; set; }
        public double Price { get; set; }
        public LimitOrderFeeModel Fee { get; set; }
    }
}