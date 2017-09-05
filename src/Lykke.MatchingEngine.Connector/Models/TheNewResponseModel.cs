using Lykke.MatchingEngine.Connector.Abstractions.Models;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class TheNewResponseModel
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }

        [ProtoMember(2, IsRequired = false)]
        public string MatchingEngineId { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public int Status { get; set; }

        [ProtoMember(4, IsRequired = false)]
        public string StatusReason { get; set; }
    }

    public static class Ext
    {
        public static MeResponseModel ToDomainModel(this TheNewResponseModel response)
        {
            return new MeResponseModel
            {
                Status = (MeStatusCodes) response.Status,
                Message = response.StatusReason,
                TransactionId = response.Id
            };
        }
    }
}