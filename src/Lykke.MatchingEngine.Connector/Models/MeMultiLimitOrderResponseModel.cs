using System.Linq;
using Lykke.MatchingEngine.Connector.Abstractions;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class MeMultiLimitOrderResponseModel
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public int Status { get; set; }

        [ProtoMember(3, IsRequired = false)]
        public string StatusReason { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public string AssetPairId { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public MeOrderStatusModel[] Statuses{ get; set; }
    }

    public static class MultiOrderExt
    {
        public static MultiLimitOrderResponse ToDomainModel(this MeMultiLimitOrderResponseModel response)
        {
            return new MultiLimitOrderResponse()
            {
                Id = response.Id,
                Status = (MeStatusCodes)response.Status,
                Message = response.StatusReason,
                TransactionId = response.Id,
                AssetPairId = response.AssetPairId,
                StatusReason = response.StatusReason,
                Statuses = response.Statuses?.Select(s => s.ToDomainModel()).ToArray() ?? new LimitOrderStatusModel[0]
            };
        }
    }
}