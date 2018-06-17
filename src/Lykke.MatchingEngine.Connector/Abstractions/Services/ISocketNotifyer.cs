using System.Threading.Tasks;

namespace Lykke.MatchingEngine.Connector.Abstractions.Services
{
    /// <summary>
    /// Даём интерфейс службам, в которых хотим обработать момент коннекта и дисконнекта
    /// </summary>
    public interface ISocketNotifier
    {
        Task Connect();
        Task Disconnect();
    }
}