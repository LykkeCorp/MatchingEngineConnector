using System;
using System.Net.Sockets;
using System.Threading;
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
        private readonly SemaphoreSlim _streamWriteLock = new SemaphoreSlim(1, 1);

        internal Action<TcpConnection> DisconnectAction;

        private readonly ITcpService _tcpService;

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
            _tcpService = tcpService;
            tcpService.SendDataToSocket = SendDataToSocket;
            _socketStatistic.LastConnectionTime = DateTime.UtcNow;
        }

        public async Task StartReadData()
        {
            if (_tcpService is ISocketNotifier socketNotifier)
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

                    if (_tcpService is ISocketNotifier socketNotifier)
                    {
                        await socketNotifier.Disconnect();
                    }

                    (_tcpService as IDisposable)?.Dispose();
                }

                _socket.Dispose();
            }
            catch (Exception exception)
            {
                _log?.Add("Disconnect error. Id=" + Id + "; Msg:" + exception.Message);
            }
        }

        public async Task<bool> SendDataToSocket(object data, CancellationToken cancellationToken)
        {
            if (Disconnected)
            {
                return false;
            }

            try
            {
                var dataToSocket = _tcpSerializer.Serialize(data);
                var stream = _socket.GetStream();
                try
                {
                    await _streamWriteLock.WaitAsync(cancellationToken);
                    await stream.WriteAsync(dataToSocket, 0, dataToSocket.Length, cancellationToken); // Concurrent write is not supported
                    await stream.FlushAsync(cancellationToken);
                    _socketStatistic.Sent += dataToSocket.Length;
                    _socketStatistic.LastSendTime = DateTime.UtcNow;
                }
                finally
                {
                    _streamWriteLock.Release();
                }

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
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
                    var (response, receivedBytes) = await _tcpSerializer.Deserialize(stream);
                    _socketStatistic.LastReceiveTime = DateTime.UtcNow;
                    _socketStatistic.Received += receivedBytes;

                    if (response != null)
                    {
                        _tcpService.HandleDataFromSocket(response);
                    }
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
            _streamWriteLock.Dispose();
        }
    }
}
