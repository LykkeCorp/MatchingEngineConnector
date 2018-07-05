using System;

namespace Lykke.MatchingEngine.Connector.Abstractions.Services
{
    [Obsolete("Do not use it. Use overloads without ISocketLog")]
    public interface ISocketLog
    {
        void Add(string message);
    }
}