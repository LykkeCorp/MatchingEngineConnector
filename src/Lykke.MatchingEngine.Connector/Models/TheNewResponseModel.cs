using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class TheNewResponseModel
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }
        [ProtoMember(2, IsRequired = false)]
        public string MeId { get; set; }
    }
}