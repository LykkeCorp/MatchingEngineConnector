using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class MeUpdateWalletCredsModel
    {
        [ProtoMember(1, IsRequired = true)]
        public long Id { get; set; }

        [ProtoMember(2, IsRequired = false)]
        public string ClientId { get; set; }


        public static MeUpdateWalletCredsModel Create(long id, string clientId)
        {
            return new MeUpdateWalletCredsModel
            {
                Id = id,
                ClientId = clientId
            };
        }
    }
}