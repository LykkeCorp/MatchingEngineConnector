using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Models;
using Lykke.MatchingEngine.Connector.Helpers;

namespace Lykke.MatchingEngine.Connector.Tools
{
    internal sealed class TcpConnection : IDisposable
    {
        private readonly ITcpSerializer _tcpSerializer;
        private readonly TcpClient _socket;
        private readonly SocketStatistic _socketStatistic;
        private readonly ISocketLog _log;
        private readonly object _disconnectLock = new object();

        internal Action<TcpConnection> DisconnectAction;

        public ITcpService TcpService { get; }

        public int Id { get; }

        public bool Disconnected { get; private set; }

        public TcpConnection(
            ITcpService tcpService,
            ITcpSerializer tcpSerializer,
            TcpClient socket,
            SocketStatistic socketStatistic,
            ISocketLog log,
            int id)
        {
            Id = id;
            _tcpSerializer = tcpSerializer;
            _socket = socket;
            _socketStatistic = socketStatistic;
            _log = log;
            TcpService = tcpService;
            tcpService.SendDataToSocket = SendDataToSocket;
            _socketStatistic.LastConnectionTime = DateTime.UtcNow;
        }

        public async Task StartReadData()
        {
            if (TcpService is ISocketNotifier socketNotifier)
            {
                await socketNotifier.Connect();
            }

            await ReadThread();
        }

        public async Task Disconnect()
        {
            try
            {
                // Вычитаем состояние дисконнект в одном потоке и сменим состояние в Disconnect=true
                bool disconnected;


                lock (_disconnectLock)
                {
                    disconnected = Disconnected;
                    Disconnected = true;
                }

                // Если вычитанное состояние не было дисконнект- почистим все что необхдимо почистить
                if (!disconnected)
                {

                    // Если серверная служба следит за тем что сокет дисконнектнулся - сообщим об этом
                    DisconnectAction?.Invoke(this);

                    _socketStatistic.LastDisconnectionTime = DateTime.UtcNow;

                    _log?.Add("Disconnected[" + Id + "]");

                    if (TcpService is ISocketNotifier socketNotifier)
                    {
                        await socketNotifier.Disconnect();
                    }

                    (TcpService as IDisposable)?.Dispose();
                }

                _socket.Dispose();
            }
            catch (Exception exception)
            {
                _log?.Add("Disconnect error. Id=" + Id + "; Msg:" + exception.Message);
            }
        }

        public async Task<bool> SendDataToSocket(object data)
        {
            if (Disconnected)
                return false;

            try
            {
                var dataToSocket = _tcpSerializer.Serialize(data);
                var stream = _socket.GetStream();
                await stream.WriteAsync(dataToSocket, 0, dataToSocket.Length);
                _socketStatistic.Sent += dataToSocket.Length;
                _socketStatistic.LastSendTime = DateTime.UtcNow;
                await stream.FlushAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Disconnect();
                _log?.Add("SendDataToSocket Error. Id: " + Id + "; Msg: " + ex.Message);
                TelemetryHelper.SubmitException(ex);
                return false;
            }
        }

        private async Task ReadThread()
        {
            try
            {
                var stream = _socket.GetStream();

                while (!Disconnected)
                {
                    var tcpData = await _tcpSerializer.Deserialize(stream);
                    _socketStatistic.LastRecieveTime = DateTime.UtcNow;
                    _socketStatistic.Recieved += tcpData.Item2;

                    if (tcpData.Item1 != null)
                        await TcpService.HandleDataFromSocket(tcpData.Item1);
                }
            }
            catch (Exception exception)
            {
                await Disconnect();
                _log?.Add($"Error ReadData [{Id}]: {exception}");
                TelemetryHelper.SubmitException(exception);
            }
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }
    }
}
