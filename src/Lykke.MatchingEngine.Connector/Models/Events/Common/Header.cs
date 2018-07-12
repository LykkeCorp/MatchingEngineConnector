using System;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Models.Events.Common
{
    [ProtoContract]
    public class Header
    {
        [ProtoMember(1, IsRequired = true)]
        public MessageType MessageType { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public long SequenceNumber { get; set; }

        /// <summary>
        /// Incoming message id
        /// </summary>
        [ProtoMember(3, IsRequired = true)]
        public string MessageId { get; set; }

        /// <summary>
        /// Incoming request id
        /// </summary>
        [ProtoMember(4, IsRequired = true)]
        public string RequestId { get; set; }

        /// <summary>
        /// Message format version
        /// </summary>
        [ProtoMember(5, IsRequired = true)]
        public string Version { get; set; }

        [ProtoMember(6, IsRequired = true, DataFormat = DataFormat.WellKnown)]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Incoming message type
        /// </summary>
        [ProtoMember(7, IsRequired = true)]
        public string EventType { get; set; }
    }
}
