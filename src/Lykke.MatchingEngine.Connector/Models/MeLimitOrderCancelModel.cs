using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class MeLimitOrderCancelModel
    {
        [ProtoMember(1, IsRequired = true)]
        public long Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public long OrderId { get; set; }


        public static MeLimitOrderCancelModel Create(long id, long orderId)
        {
            return new MeLimitOrderCancelModel
            {
                Id = id,
                OrderId = orderId
            };
        }

    }
}