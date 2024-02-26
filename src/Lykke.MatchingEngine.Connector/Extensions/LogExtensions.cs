using System;
using Microsoft.Extensions.Logging;

namespace Lykke.MatchingEngine.Connector.Extensions;

internal static partial class LogExtensions
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Sent data {MeData}")]
    public static partial void SentData(this ILogger logger, object meData);
    [LoggerMessage(Level = LogLevel.Debug, Message = "Received data: {MeData}, last receive time: {LastReceiveTime}")]
    public static partial void ReceivedData(this ILogger logger, object meData, DateTime lastReceiveTime);
}
