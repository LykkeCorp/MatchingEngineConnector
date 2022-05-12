using System;
using System.Net;

namespace Lykke.MatchingEngine.Connector.Services
{
    /// <summary>
    /// settings for MatchingEngine client
    /// </summary>
    public class MeClientSettings
    {
        /// <summary>
        /// ME IP endpoint
        /// </summary>
        public IPEndPoint Endpoint { get; set; }

        /// <summary>
        /// Flag indicating, if unknown response error should not rise exception
        /// </summary>
        public bool IgnoreErrors { get; set; }

        /// <summary>
        /// Interval for ping package. Default 5 sec.
        /// </summary>
        public TimeSpan PingInterval { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Interval to disconnect. Default 10 sec.
        /// </summary>
        public TimeSpan DisconnectInterval { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Delay before reconnect. Default 3 sec.
        /// </summary>
        public TimeSpan ReconnectTimeOut { get; set; } = TimeSpan.FromSeconds(3);

        /// <summary>
        /// Enable retries on ME operations
        /// </summary>
        public bool EnableRetries { get; set; }

        /// <summary>
        /// Should log ME response
        /// </summary>
        public bool LogResponse { get; set; } = true;
    }
}
