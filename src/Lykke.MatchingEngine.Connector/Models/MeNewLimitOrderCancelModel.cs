using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class MeNewLimitOrderCancelModel
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string[] LimitOrderId { get; set; }

        public static MeNewLimitOrderCancelModel Create(string id, string[] limitOrderId)
        {
            return new MeNewLimitOrderCancelModel
            {
                Id = id,
                LimitOrderId = limitOrderId
            };
        }

    }
}