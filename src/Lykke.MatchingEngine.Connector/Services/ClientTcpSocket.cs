using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Extensions;
using Lykke.MatchingEngine.Connector.Models.Me;
using Lykke.MatchingEngine.Connector.Tools;
using Microsoft.Extensions.Logging;

namespace Lykke.MatchingEngine.Connector.Services
{
    internal class ClientTcpSocket<TTcpSerializer, TService> : IClientSocketConsumer<TService>
        where TTcpSerializer : ITcpSerializer, new()
        where TService : class, ITcpClientService
    {
        private readonly int _pingInterval = 5;
        private readonly int _disconnectInterval = 10;

        private readonly ILoggerFactory _loggerFactory;
        private readonly IPEndPoint _ipEndPoint;
        private readonly int _reconnectTimeOut;
        private readonly Func<TService> _srvFactory;
        private readonly ILogger<ClientTcpSocket<TTcpSerializer, TService>> _logger;

        private int _id;
        private bool _working;
        private TService _service;

        public bool Connected => _service != null;

        public SocketStatistic SocketStatistic { get; }

        public ClientTcpSocket(
            ILoggerFactory loggerFactory,
            IPEndPoint ipEndPoint,
            int reconnectTimeOut,
            Func<TService> srvFactory)
        {
            SocketStatistic = new SocketStatistic();
            _logger = loggerFactory.CreateLogger<ClientTcpSocket<TTcpSerializer, TService>>();
            _loggerFactory = loggerFactory;
            _ipEndPoint = ipEndPoint;
            _reconnectTimeOut = reconnectTimeOut;

            _srvFactory = srvFactory;
        }

        public ClientTcpSocket(
            ILoggerFactory loggerFactory,
            MeClientSettings settings,
            Func<TService> srvFactory)
        {
            SocketStatistic = new SocketStatistic();
            _logger = loggerFactory.CreateLogger<ClientTcpSocket<TTcpSerializer, TService>>();
            _loggerFactory = loggerFactory;
            _ipEndPoint = settings.Endpoint;
            _reconnectTimeOut = (int)settings.ReconnectTimeOut.TotalMilliseconds;
            _pingInterval = (int)settings.PingInterval.TotalSeconds;
            _disconnectInterval = (int) settings.DisconnectInterval.TotalSeconds;
            _srvFactory = srvFactory;
        }

        private async void SocketThread()
        {
            while (_working)
            {
                try
                {
                    using (var connection = await Connect())
                    {
                        var readTask = Task.Factory.StartNew(connection.StartReadData, TaskCreationOptions.LongRunning).Unwrap();
                        var pingTask = Task.Factory.StartNew(() => SocketPingProcess(connection), TaskCreationOptions.LongRunning).Unwrap();
                        await Task.WhenAny(readTask, pingTask);
                    }
                }
                catch (SocketException se)
                {
                    if (se.SocketErrorCode == SocketError.ConnectionRefused)
                    {
                        _logger.LogError(se, "Connection support exception");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Connection support fatal exception");
                }
                finally
                {
                    _service = default(TService);
                    _logger.LogWarning("Connection timeout...");
                    Thread.Sleep(_reconnectTimeOut);
                }
            }
        }

        public void Start()
        {
            if (_working)
                throw new Exception("Client socket has already started");
            
            _working = true;
            SocketThread();
        }

        TService IClientSocketConsumer<TService>.GetConnection()
        {
            return _service;
        }

        private async Task<TcpConnection> Connect()
        {
            _logger.LogDebug("Attempt to connect {@Address}", new
            {
                address = _ipEndPoint.Address.ToString(),
                port = _ipEndPoint.Port
            });

            var tcpClient = new TcpClient { NoDelay = true };
            
            try
            {
                await tcpClient.ConnectAsync(_ipEndPoint.Address, _ipEndPoint.Port);
            }
            catch (Exception)
            {
                tcpClient.Dispose();
                throw;
            }
            
            _service = _srvFactory();
            SocketStatistic.Init();
            var tcpSerializer = new TTcpSerializer();

            var connection = new TcpConnection(_service, tcpSerializer, tcpClient, SocketStatistic, _loggerFactory.CreateLogger<TcpConnection>(), _id++);

            _logger.LogDebug("Connected {ConnectionId}", connection.Id);

            return connection;
        }

        private async Task SocketPingProcess(TcpConnection connection)
        {
            var lastSendPingTime = DateTime.UtcNow;
            
            try
            {
                while (!connection.Disconnected)
                {
                    await Task.Delay(500); 
                    
                    if ((DateTime.UtcNow - SocketStatistic.LastReceiveTime).TotalSeconds > _disconnectInterval)
                    {
                        _logger.LogWarning("There is no receive activity during {DisconnectInterval} seconds. Disconnecting...", _disconnectInterval);
                        await connection.Disconnect(new Exception($"There is no receive activity during {_disconnectInterval} seconds. Disconnecting..."));
                    }
                    else if ((DateTime.UtcNow - lastSendPingTime).TotalSeconds > _pingInterval)
                    {
                        var pingData = _service.GetPingData();
                        _logger.SendingPingData(pingData);
                        await connection.SendDataToSocket(pingData, CancellationToken.None);
                        lastSendPingTime = DateTime.UtcNow;
                    }
                }
            }
            catch (Exception exception)
            {
                await connection.Disconnect(exception);
                _logger.LogError(exception, "Ping Thread Exception");
            }
        }
    }
}
