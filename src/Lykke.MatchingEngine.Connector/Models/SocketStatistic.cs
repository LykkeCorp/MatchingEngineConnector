using System;

namespace Lykke.MatchingEngine.Connector.Models
{
    public class SocketStatistic
    {

        public void Init()
        {
            LastConnectionTime = DateTime.UtcNow;
            LastReceiveTime = DateTime.UtcNow;
            LastSendTime = DateTime.UtcNow;
            LastDisconnectionTime = DateTime.UtcNow;
        }

        public DateTime LastConnectionTime { get; set; }
        public DateTime LastSendTime { get; set; }
        public DateTime LastReceiveTime { get; set; }
        public DateTime LastDisconnectionTime { get; set; }

        public long Sent { get; set; }

        public long Received { get; set; }


    }
}
