namespace Lykke.MatchingEngine.Connector.Abstractions.Models
{
    public class LimitOrderFeeModel
    {
        public int Type { get; set; }
        public double MakerSize { get; set; }
        public double TakerSize { get; set; }
        public string SourceClientId { get; set; }
        public string TargetClientId { get; set; }
    }
}
