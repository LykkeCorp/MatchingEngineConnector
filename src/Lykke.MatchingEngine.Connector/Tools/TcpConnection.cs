using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Models.Me;
using Lykke.MatchingEngine.Connector.Helpers;

namespace Lykke.MatchingEngine.Connector.Tools
{
    internal sealed class TcpConnection : IDisposable
    {
        private readonly ITcpSerializer _tcpSerializer;
        private readonly TcpClient _socket;
        private readonly SocketStatistic _socketStatistic;
        [Obsolete]
        [CanBeNull]
        private readonly ISocketLog _legacyLog;
        private readonly object _disconnectLock = new object();
        private readonly SemaphoreSlim _streamWriteLock = new SemaphoreSlim(1, 1);
        [CanBeNull]
        private readonly ILog _log;

        internal Action<TcpConnection> DisconnectAction;

        private readonly ITcpService _tcpService;
        
        public int Id { get; }

        public bool Disconnected { get; private set; }

        [Obsolete]
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
            _legacyLog = log;
            _tcpService = tcpService;
            tcpService.SendDataToSocket = SendDataToSocket;
            _socketStatistic.LastConnectionTime = DateTime.UtcNow;
        }

        public TcpConnection(
            ITcpService tcpService,
            ITcpSerializer tcpSerializer,
            TcpClient socket,
            SocketStatistic socketStatistic,
            ILogFactory logFactory,
            int id)
        {
            Id = id;
            _tcpSerializer = tcpSerializer;
            _socket = socket;
            _socketStatistic = socketStatistic;
            _log = logFactory.CreateLog(this);
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

        public async Task Disconnect(Exception exc)
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

                    _log?.Info("Disconnected", new {connectionId = Id});
                    _legacyLog?.Add("Disconnected[" + Id + "]");

                    if (_tcpService is ISocketNotifier socketNotifier)
                    {
                        await socketNotifier.Disconnect(exc);
                    }

                    (_tcpService as IDisposable)?.Dispose();
                }

                _socket.Dispose();
            }
            catch (Exception exception)
            {
                _log?.Error(exception, "Disconnection error", new {connectionId = Id});
                _legacyLog?.Add("Disconnect error. Id=" + Id + "; Msg:" + exception.Message);
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
                await Disconnect(ex);

                _log?.Error(ex, context: new {connectionId = Id});
                _legacyLog?.Add("SendDataToSocket Error. Id: " + Id + "; Msg: " + ex.Message);
                
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
                await Disconnect(exception);

                _log?.Error(exception, context: new {connectionId = Id});
                _legacyLog?.Add($"Error ReadData [{Id}]: {exception}");
                
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
