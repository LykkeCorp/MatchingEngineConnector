namespace Lykke.MatchingEngine.Connector.Abstractions.Models
{
    public class MarketOrderModel
    {
        public string Id  { get; set; }
        public string ClientId  { get; set; }
        public string AssetPairId { get; set; }
        public OrderAction OrderAction { get; set; }
        public double Volume { get; set; }
        public bool Straight { get; set; }
        public double? ReservedLimitVolume { get; set; }
        public MarketOrderFeeModel Fee { get; set; }
    }
}
