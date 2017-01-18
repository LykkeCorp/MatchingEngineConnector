using System;
using System.IO;
using System.Threading.Tasks;

namespace Lykke.MatchingEngine.Connector.Tools
{
    /// <summary>
    /// Интерфейс, который выдается сериалайзеру данных
    /// </summary>
    public interface ITcpSerializer
    {
        Task<Tuple<object, int>> Deserialize(Stream stream);
        byte[] Serialize(object data);
    }
}