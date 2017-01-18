using System.Threading.Tasks;

namespace Lykke.MatchingEngine.Connector.Abstractions.Services
{
    /// <summary>
    /// Даём интерфейс службам, в которых хотим обработать момент коннекта и дисконнекта
    /// </summary>
    public interface ISocketNotifyer
    {
        Task Connect();
        Task Disconnect();
    }
}