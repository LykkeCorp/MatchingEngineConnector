using System;
using System.Collections.Generic;

namespace Lykke.MatchingEngine.Connector.Messages
{
    public class OrderBook
    {
        public string AssetPair { get; set; }
        public bool IsBuy { get; set; }
        public DateTime Timestamp { get; set; }
        public IReadOnlyList<PriceVolume> Prices { get; set; }
    }

    public class PriceVolume
    {
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
    }
}
