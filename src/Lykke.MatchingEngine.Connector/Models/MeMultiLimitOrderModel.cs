using Common;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class MeMultiLimitOrderModel
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public long DateTime { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public string ClientId { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public string AssetId { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public MeMultiOrderItemModel[] Orders { get; set; }

        [ProtoMember(6, IsRequired = false)]
        public bool CancelAllPreviousLimitOrders { get; set; }

        public static MeMultiLimitOrderModel Create(string id, string clientId, string assetId, MeMultiOrderItemModel[] orders, bool cancelAllPreviousLimitOrders = false)
        {
            return new MeMultiLimitOrderModel
            {
                Id = id,
                DateTime = (long)System.DateTime.UtcNow.ToUnixTime(),
                ClientId = clientId,
                AssetId = assetId,
                Orders = orders ?? new MeMultiOrderItemModel[0],
                CancelAllPreviousLimitOrders = cancelAllPreviousLimitOrders
            };
        }
    }
}