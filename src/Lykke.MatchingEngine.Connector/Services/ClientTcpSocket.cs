using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Models.Me;
using Lykke.MatchingEngine.Connector.Tools;

namespace Lykke.MatchingEngine.Connector.Services
{
    internal class ClientTcpSocket<TTcpSerializer, TService> : IClientSocketConsumer<TService>
        where TTcpSerializer : ITcpSerializer, new()
        where TService : class, ITcpClientService
    {
        public const int PingInterval = 5;

        [Obsolete]
        [CanBeNull]
        private readonly ISocketLog _legacyLog;
        private readonly IPEndPoint _ipEndPoint;
        private readonly int _reconnectTimeOut;
        private readonly Func<TService> _srvFactory;
        [CanBeNull]
        private readonly ILog _log;
        [CanBeNull]
        private readonly ILogFactory _logFactory;

        private int _id;
        private bool _working;
        private TService _service;
        


        public bool Connected => _service != null;

        public SocketStatistic SocketStatistic { get; }

        [Obsolete]
        public ClientTcpSocket(
            ISocketLog log,
            IPEndPoint ipEndPoint,
            int reconnectTimeOut,
            Func<TService> srvFactory)
        {
            SocketStatistic = new SocketStatistic();
            _legacyLog = log;
            _ipEndPoint = ipEndPoint;
            _reconnectTimeOut = reconnectTimeOut;

            _srvFactory = srvFactory;
        }

        public ClientTcpSocket(
            ILogFactory logFactory,
            IPEndPoint ipEndPoint,
            int reconnectTimeOut,
            Func<TService> srvFactory)
        {
            SocketStatistic = new SocketStatistic();
            _logFactory = logFactory;
            _log = logFactory.CreateLog(this);
            _ipEndPoint = ipEndPoint;
            _reconnectTimeOut = reconnectTimeOut;

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
                        _log?.Error(se, "Connection support exception");
                        _legacyLog?.Add("Connection support exception: " + se.Message);
                    }
                }
                catch (Exception ex)
                {
                    _log?.Error(ex, "Connection support fatal exception");
                    _legacyLog?.Add("Connection support fatal exception:" + ex.Message);
                }
                finally
                {
                    _service = default(TService);
                    _log?.Error("Connection timeout...");
                    _legacyLog?.Add("Connection Timeout...");
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
            _log?.Debug("Attempt to connect", new
            {
                address = _ipEndPoint.Address,
                port = _ipEndPoint.Port
            });
            _legacyLog?.Add("Attempt To Connect:" + _ipEndPoint.Address + ":" + _ipEndPoint.Port);

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

            if (_logFactory != null)
            {
                var connection = new TcpConnection(_service, tcpSerializer, tcpClient, SocketStatistic, _logFactory, _id++);
                
                _log.Debug("Connected", new {connectionId = connection.Id});

                return connection;
            }
            else
            {
                var connection = new TcpConnection(_service, tcpSerializer, tcpClient, SocketStatistic, _legacyLog, _id++);
                
                _legacyLog?.Add("Connected. Id=" + connection.Id);

                return connection;
            }
        }

        private async Task SocketPingProcess(TcpConnection connection)
        {
            var lastSendPingTime = DateTime.UtcNow;
            try
            {
                while (!connection.Disconnected)
                {
                    await Task.Delay(500);

                    if ((DateTime.UtcNow - SocketStatistic.LastReceiveTime).TotalSeconds > PingInterval * 2)
                    {
                        _log?.Error(
                            message: "There is no receive activity for a too long time. Disconnecting...",
                            context: new
                            {
                                pingInterval = PingInterval * 2
                            });
                        _legacyLog?.Add("Long time [" + PingInterval * 2 + "] no receive activity. Disconnect...");
                        await connection.Disconnect();
                    }
                    else if ((DateTime.UtcNow - lastSendPingTime).TotalSeconds > PingInterval)
                    {
                        var pingData = _service.GetPingData();
                        await connection.SendDataToSocket(pingData, CancellationToken.None);
                        lastSendPingTime = DateTime.UtcNow;
                    }
                }
            }
            catch (Exception exception)
            {
                await connection.Disconnect();

                _log?.Error(exception);
                _legacyLog?.Add("Ping Thread Exception: " + exception.Message);
            }
        }
    }
}
