using Common;
using Lykke.MatchingEngine.Connector.Models.Api;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Me
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

        [ProtoMember(7, IsRequired = false)]
        public int CancelMode { get; set; }

        public static MeMultiLimitOrderModel Create(
            string id,
            string clientId,
            string assetId,
            MeMultiOrderItemModel[] orders,
            bool cancelAllPreviousLimitOrders = false,
            CancelMode cancelMode = default)
        {
            return new MeMultiLimitOrderModel
            {
                Id = id,
                DateTime = (long) System.DateTime.UtcNow.ToUnixTime(),
                ClientId = clientId,
                AssetId = assetId,
                Orders = orders ?? new MeMultiOrderItemModel[0],
                CancelAllPreviousLimitOrders = cancelAllPreviousLimitOrders,
                CancelMode = (int) cancelMode
            };
        }
    }
}