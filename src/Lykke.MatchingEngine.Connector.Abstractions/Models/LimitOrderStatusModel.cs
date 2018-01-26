using Lykke.MatchingEngine.Connector.Abstractions.Models;

namespace Lykke.MatchingEngine.Connector.Abstractions
{
    public class LimitOrderStatusModel
    {
        public string Id { get; set; }
        public string MatchingEngineId { get; set; }
        public MeStatusCodes Status { get; set; }
        public string StatusReason { get; set; }
        public double Volume { get; set; }
        public double Price { get; set; }
    }
}