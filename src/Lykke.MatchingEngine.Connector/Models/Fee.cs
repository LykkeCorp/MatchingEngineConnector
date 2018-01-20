using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class Fee
    {
        [ProtoMember(1, IsRequired = true)]
        public int Type { get; set; }

        [ProtoMember(2, IsRequired = false)]
        public double Size { get; set; }

        [ProtoMember(3, IsRequired = false)]
        public string SourceClientId { get; set; }

        [ProtoMember(4, IsRequired = false)]
        public string TargetClientId { get; set; }

        [ProtoMember(5, IsRequired = false)]
        public int SizeType { get; set; }
    }

}
