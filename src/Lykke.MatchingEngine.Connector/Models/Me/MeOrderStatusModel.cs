using Lykke.MatchingEngine.Connector.Abstractions;
using Lykke.MatchingEngine.Connector.Models.Api;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Me
{
    [ProtoContract]
    public class MeOrderStatusModel
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }

        [ProtoMember(2, IsRequired = false)]
        public string MatchingEngineId { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public int Status { get; set; }

        [ProtoMember(4, IsRequired = false)]
        public string StatusReason { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public double Volume { get; set; }

        [ProtoMember(6, IsRequired = true)]
        public double Price { get; set; }
    }

    public static class OrderStatusExt
    {
        public static LimitOrderStatusModel ToDomainModel(this MeOrderStatusModel response)
        {
            return new LimitOrderStatusModel
            {
                Status = (MeStatusCodes)response.Status,
                StatusReason = response.StatusReason,
                Id = response.Id,
                MatchingEngineId = response.MatchingEngineId,
                Price = response.Price,
                Volume = response.Volume
            };
        }
    }
}