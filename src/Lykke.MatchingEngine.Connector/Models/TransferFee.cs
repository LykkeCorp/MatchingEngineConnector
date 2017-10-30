using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class TransferFee
    {
        [ProtoMember(1, IsRequired = true)]
        public int Type { get; set; }

        [ProtoMember(2, IsRequired = false)]
        public double SizePercantage { get; set; }

        [ProtoMember(3, IsRequired = false)]
        public string SourceClientId { get; set; }

        [ProtoMember(4, IsRequired = false)]
        public string TargetClientId { get; set; }
    }

    public enum TransferFeeType
    {
        NO_FEE = 0,
        CLIENT_FEE = 1,
        EXTERNAL_FEE = 2
    }
}
