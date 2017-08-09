namespace Lykke.MatchingEngine.Connector.Abstractions.Models
{
    public class MarketOrderResponse
    {
        public MeStatusCodes Status { get; set; }
        public double Price { get; set; }
    }
}
