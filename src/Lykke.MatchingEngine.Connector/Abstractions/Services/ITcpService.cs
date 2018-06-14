using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.MatchingEngine.Connector.Abstractions.Services
{
    /// <summary>
    /// Интерфейс, с помощью которого мы уведомляем класс потребителя информации о поступлении данных 
    /// и даём интерфейс (SendDataToSocket) на обратную отправку данных
    /// </summary>
    public interface ITcpService
    {
        /// <summary>
        /// Когда сокет получил данные и распарсил их в объект, 
        /// вызывается этот метод, в котором мы обрабатываем полученные данные
        /// </summary>
        /// <param name="data">данные, которые получил сокет и распарсил биндер</param>
        void HandleDataFromSocket(object data);

        /// <summary>
        /// Метод, с помощью которого мы отправляем данные в сокет
        /// </summary>

        Func<object, CancellationToken, Task<bool>> SendDataToSocket { get; set; }

        /// <summary>
        /// Имя контекста сокета (для логирования)
        /// </summary>
        string ContextName { get; }

    }
}