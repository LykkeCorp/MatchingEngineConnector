using System;
using Microsoft.Extensions.Logging;

namespace Lykke.MatchingEngine.Connector.Extensions;

internal static partial class LogExtensions
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Sending ping data {PingData}")]
    public static partial void SendingPingData(this ILogger logger, object pingData);
    [LoggerMessage(Level = LogLevel.Debug, Message = "Received data: {MeData}, last receive time: {LastReceiveTime}")]
    public static partial void ReceivedData(this ILogger logger, object meData, DateTime lastReceiveTime);
}
