using System;

namespace Lykke.MatchingEngine.Connector.Abstractions.Models
{
    public class MarketOrderFeeModel
    {
        public int Type { get; set; }
        public double Size { get; set; }
        public string SourceClientId { get; set; }
        public string TargetClientId { get; set; }
        public int SizeType { get; set; }
        public string[] AssetId { get; set; } = Array.Empty<string>();
    }

    public enum MarketOrderFeeType
    {
        NO_FEE = 0,
        CLIENT_FEE = 1,
        EXTERNAL_FEE = 2
    }
}
