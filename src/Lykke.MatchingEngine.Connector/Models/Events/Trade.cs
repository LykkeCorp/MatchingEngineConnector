using System;
using System.Collections.Generic;
using Lykke.MatchingEngine.Connector.Models.Events.Common;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Events
{
    [ProtoContract]
    public class Trade
    {
        [ProtoMember(1, IsRequired = true)]
        public string TradeId { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string AssetId { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public string Volume { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public string Price { get; set; }

        [ProtoMember(5, IsRequired = true, DataFormat = DataFormat.WellKnown)]
        public DateTime Timestamp { get; set; }

        [ProtoMember(6, IsRequired = true)]
        public string OppositeOrderId { get; set; }

        [ProtoMember(7, IsRequired = true)]
        public string OppositeExternalOrderId { get; set; }

        [ProtoMember(8, IsRequired = true)]
        public string OppositeWalletId { get; set; }

        [ProtoMember(9, IsRequired = true)]
        public string OppositeAssetId { get; set; }

        [ProtoMember(10, IsRequired = true)]
        public string OppositeVolume { get; set; }

        [ProtoMember(11, IsRequired = true)]
        public int Index { get; set; }

        [ProtoMember(12, IsRequired = false)]
        public string AbsoluteSpread { get; set; }

        [ProtoMember(13, IsRequired = false)]
        public string RelativeSpread { get; set; }

        [ProtoMember(14, IsRequired = true)]
        public TradeRole Role { get; set; }

        [ProtoMember(15, IsRequired = false)]
        public List<FeeTransfer> Fees { get; set; }
    }
}
