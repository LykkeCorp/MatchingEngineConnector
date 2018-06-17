using Lykke.MatchingEngine.Connector.Models.Common;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Me
{
    [ProtoContract]
    public class MeMultiOrderItemModel
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public double Volume { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public double Price { get; set; }

        [ProtoMember(4, IsRequired = false)]
        public MeLimitOrderFeeModel Fee { get; set; }

        [ProtoMember(6, IsRequired = false)]
        public string OldUid { get; set; }

        public static MeMultiOrderItemModel Create(
            string id,
            OrderAction orderAction,
            double volume,
            double price,
            MeLimitOrderFeeModel fee,
            string oldUid)
        {
            return new MeMultiOrderItemModel
            {
                Id = id,
                Volume = orderAction == OrderAction.Buy ? volume : -volume,
                Price = price,
                Fee = fee,
                OldUid = oldUid
            };
        }
    }
}