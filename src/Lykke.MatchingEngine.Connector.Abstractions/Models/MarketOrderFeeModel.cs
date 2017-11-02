namespace Lykke.MatchingEngine.Connector.Abstractions.Models
{
    public class MarketOrderFeeModel
    {
        public int Type { get; set; }
        public double Size { get; set; }
        public string SourceClientId { get; set; }
        public string TargetClientId { get; set; }
    }
}
