using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Me
{
    [ProtoContract]
    public class TheResponseModel
    {
        [ProtoMember(1, IsRequired = true)]
        public long ProcessId { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string CorrelationId { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public string RecordId { get; set; }
    }
}