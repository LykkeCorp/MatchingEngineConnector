using Common;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class MeLimitOrderModel
    {
        [ProtoMember(1, IsRequired = true)]
        public long Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public long DateTime { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public string ClientId { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public string AssetId { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public double Volume { get; set; }

        [ProtoMember(6, IsRequired = true)]
        public double Price { get; set; }

        public static MeLimitOrderModel Create(long id, string clientId, string assetId, OrderAction orderAction, double volume, double price)
        {
            return new MeLimitOrderModel
            {
                Id = id,
                DateTime = (long)System.DateTime.UtcNow.ToUnixTime(),
                ClientId = clientId,
                AssetId = assetId,
                Volume = orderAction == OrderAction.Buy ? volume : -volume,
                Price = price
            };
        }
    }
}