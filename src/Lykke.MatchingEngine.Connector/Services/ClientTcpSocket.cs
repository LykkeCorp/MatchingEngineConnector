using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Models;
using Lykke.MatchingEngine.Connector.Tools;

namespace Lykke.MatchingEngine.Connector.Services
{
    internal class ClientTcpSocket<TTcpSerializer, TService> : IClientSocketConsumer<TService>
        where TTcpSerializer : ITcpSerializer, new()
        where TService : class, ITcpClientService
    {
        public const int PingInterval = 5;

        private readonly ISocketLog _log;
        private readonly IPEndPoint _ipEndPoint;
        private readonly int _reconnectTimeOut;
        private readonly Func<TService> _srvFactory;

        private int _id;
        private bool _working;
        private TService _service;

        public bool Connected => _service != null;

        public SocketStatistic SocketStatistic { get; }

        public ClientTcpSocket(
            ISocketLog log,
            IPEndPoint ipEndPoint,
            int reconnectTimeOut,
            Func<TService> srvFactory)
        {
            SocketStatistic = new SocketStatistic();
            _log = log;
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
                        _log?.Add("Connection support exception: " + se.Message);
                }
                catch (Exception ex)
                {
                    _log?.Add("Connection support fatal exception:" + ex.Message);
                }
                finally
                {
                    _service = default(TService);
                    _log?.Add("Connection Timeout...");
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
            _log?.Add("Attempt To Connect:" + _ipEndPoint.Address + ":" + _ipEndPoint.Port);

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
            var connection = new TcpConnection(_service, tcpSerializer, tcpClient, SocketStatistic, _log, _id++);

            _log?.Add("Connected. Id=" + connection.Id);

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

                    if ((DateTime.UtcNow - SocketStatistic.LastReceiveTime).TotalSeconds > PingInterval * 2)
                    {
                        _log?.Add("Long time [" + PingInterval * 2 + "] no receive activity. Disconnect...");
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
                _log?.Add("Ping Thread Exception: " + exception.Message);
            }
        }
    }
}
