using Common;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using ProtoBuf;
using System;

namespace Lykke.MatchingEngine.Connector.Models
{
    [ProtoContract]
    public class MeNewCashInOutModel
    {
        [ProtoMember(1, IsRequired = true)]
        public string Id { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string ClientId { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public long DateTime { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public string AssetId { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public double Amount { get; set; }

        [ProtoMember(6, IsRequired = false)]
        public Fee Fee { get; set; }
        
        public static MeNewCashInOutModel Create(string id, string clientId,
            string assetId, double amount, string feeClientId, double feeSize, FeeSizeType feeSizeType)
        {
            var feeAbsolute = 0.0;

            if (feeSize > 0)
            {
                if (feeSizeType == FeeSizeType.ABSOLUTE)
                    feeAbsolute = feeSize;
                if (feeSizeType == FeeSizeType.PERCENTAGE)
                    feeAbsolute = Math.Round(amount * feeSize, 15);
            }

            return new MeNewCashInOutModel
            {
                Id = id,
                ClientId = clientId,
                DateTime = (long)System.DateTime.UtcNow.ToUnixTime(),
                AssetId = assetId,
                Amount = amount + feeAbsolute,
                Fee = new Fee()
                {
                    SourceClientId = null,
                    TargetClientId = feeClientId,
                    Size = feeAbsolute,
                    Type = (int)FeeType.CLIENT_FEE,
                    SizeType = (int)feeSizeType
                }
            };
        }
    }
}
