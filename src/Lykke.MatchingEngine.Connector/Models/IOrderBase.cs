using System;

namespace Lykke.MatchingEngine.Connector.Models
{
    public interface IOrderBase
    {
        string Id { get; set; }
        string ClientId { get; set; }
        DateTime CreatedAt { get; set; }
        double Volume { get; set; }
        double Price { get; set; }
        string AssetPairId { get; set; }
        string Status { get; set; }
        bool Straight { get; set; }
    }
}