using System;
using System.IO;
using System.Threading.Tasks;

namespace Lykke.MatchingEngine.Connector.Tools
{
    /// <summary>
    /// Интерфейс, который выдается сериалайзеру данных
    /// </summary>
    internal interface ITcpSerializer
    {
        Task<(object response, int receivedBytes)> Deserialize(Stream stream);
        byte[] Serialize(object data);
    }
}