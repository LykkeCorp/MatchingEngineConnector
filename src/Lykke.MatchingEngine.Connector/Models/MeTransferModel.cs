﻿using Common;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class MeTransferModel
    {
        [ProtoMember(1, IsRequired = true)]
        public long Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string FromClientId { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public string ToClientId { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public long DateTime { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public string AssetId { get; set; }

        [ProtoMember(6, IsRequired = true)]
        public double Amount { get; set; }

        [ProtoMember(7, IsRequired = true)]
        public string BusinessId { get; set; }

        public static MeTransferModel Create(long id, string fromClientId,
            string toClientId, string assetId, double amount, string businessId)
        {
            return new MeTransferModel
            {
                Id = id,
                FromClientId = fromClientId,
                ToClientId = toClientId,
                DateTime = (long)System.DateTime.UtcNow.ToUnixTime(),
                AssetId = assetId,
                Amount = amount,
                BusinessId = businessId
            };
        }
    }
}