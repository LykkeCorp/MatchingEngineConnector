using System;
using Lykke.MatchingEngine.Connector.Abstractions.Services;

namespace Lykke.MatchingEngine.Connector.Services
{
    [Obsolete("Do not use it. Use overloads without ISocketLog")]
    public class SocketLogDynamic : ISocketLog
    {
        private readonly Action<int> _changeCount;
        private readonly Action<string> _newMessage;

        public SocketLogDynamic(Action<int> changeCount, Action<string> newMessage)
        {
            _changeCount = changeCount;
            _newMessage = newMessage;
        }

        public void Add(string message)
        {
            _newMessage(message);
        }

    }
}