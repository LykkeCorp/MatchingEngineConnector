using System;
using System.Collections.Generic;
using Lykke.MatchingEngine.Connector.Models.Events.Common;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Events
{
    [ProtoContract]
    public class Order
    {
        [ProtoMember(1, IsRequired = true)]
        public OrderType OrderType { get; set; }

        /// <summary>
        /// Matching engine id
        /// </summary>
        [ProtoMember(2, IsRequired = true)]
        public string Id { get; set; }

        /// <summary>
        /// External Id
        /// </summary>
        [ProtoMember(3, IsRequired = true)]
        public string ExternalId { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public string AssetPairId { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public string WalletId { get; set; }

        [ProtoMember(6, IsRequired = true)]
        public OrderSide Side { get; set; }

        /// <summary>
        /// Positive number
        /// </summary>
        [ProtoMember(7, IsRequired = true)]
        public string Volume { get; set; }

        /// <summary>
        /// Positive number
        /// </summary>
        [ProtoMember(8, IsRequired = false)]
        public string RemainingVolume { get; set; }

        /// <summary>
        /// Could be empty for market orders
        /// </summary>
        [ProtoMember(9, IsRequired = false)]
        public string Price { get; set; }

        [ProtoMember(10, IsRequired = true)]
        public OrderStatus Status { get; set; }

        /// <summary>
        /// Any string status, current possible values:
        /// NOT_ENOUGH_FUNDS
        /// RESERVED_VOLUME_GREATER_THAN_BALANCE
        /// NO_LIQUIDITY
        /// UNKNOWN_ASSET
        /// DISABLED_ASSET
        /// LEAD_TO_NEGATIVE_SPREAD
        /// INVALID_FEE
        /// TOO_SMALL_VOLUME
        /// INVALID_PRICE
        /// NOT_FOUND_PREVIOUS
        /// INVALID_PRICE_ACCURACY
        /// INVALID_VOLUME_ACCURACY
        /// </summary>
        [ProtoMember(11, IsRequired = false)]
        public string RejectReason { get; set; }

        /// <summary>
        /// Last status date
        /// </summary>
        [ProtoMember(12, IsRequired = true, DataFormat = DataFormat.WellKnown)]
        public DateTime StatusDate { get; set; }

        /// <summary>
        /// Date from incoming request
        /// </summary>
        [ProtoMember(13, IsRequired = true, DataFormat = DataFormat.WellKnown)]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date of first processing in ME
        /// </summary>
        [ProtoMember(14, IsRequired = true, DataFormat = DataFormat.WellKnown)]
        public DateTime Registered { get; set; }

        /// <summary>
        /// Date of last matching
        /// </summary>
        [ProtoMember(15, IsRequired = false, DataFormat = DataFormat.WellKnown)]
        public DateTime? LastMatchTime { get; set; }

        /// <summary>
        /// StopLimit orders
        /// </summary>
        [ProtoMember(16, IsRequired = false)]
        public string LowerLimitPrice { get; set; }

        /// <summary>
        /// StopLimit orders
        /// </summary>
        [ProtoMember(17, IsRequired = false)]
        public string LowerPrice { get; set; }

        /// <summary>
        /// StopLimit orders
        /// </summary>
        [ProtoMember(18, IsRequired = false)]
        public string UpperLimitPrice { get; set; }

        /// <summary>
        /// StopLimit orders
        /// </summary>
        [ProtoMember(19, IsRequired = false)]
        public string UpperPrice { get; set; }

        /// <summary>
        /// Market orders
        /// </summary>
        [ProtoMember(20, IsRequired = false)]
        public bool? Straight { get; set; }

        [ProtoMember(21, IsRequired = false)]
        public List<FeeInstruction> Fees { get; set; }

        [ProtoMember(22, IsRequired = false)]
        public List<Trade> Trades { get; set; }
    }
}
