using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Models;

namespace Lykke.MatchingEngine.Connector.Tools
{
    /// <summary>
    /// Class wich reads data during established connection 
    /// </summary>
    public class TcpConnection
    {
        private readonly ITcpSerializer _tcpSerializer;
        private readonly TcpClient _socket;
        private readonly SocketStatistic _socketStatistic;
        private readonly ISocketLog _log;
        internal Action<TcpConnection> DisconnectAction;

        public TcpConnection(ITcpService tcpService,
            ITcpSerializer tcpSerializer, TcpClient socket, SocketStatistic socketStatistic, ISocketLog log, int id)
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

        public ITcpService TcpService { get; }


        public int Id { get; }

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
                _log.Add("Error ReadData [" + Id + "]:" + exception.Message);
            }

        }

        public async Task StartReadData()
        {
            var socketNotifier = TcpService as ISocketNotifyer;
            if (socketNotifier != null)
                await socketNotifier.Connect();

            await ReadThread();
        }

        public bool Disconnected { get; private set; }

        private readonly object _lockObject = new object();

        public async Task Disconnect()
        {
            try
            {

                // Вычитаем состояние дисконнект в одном потоке и сменим состояние в Disconnect=true
                bool disconnected;

                lock (_lockObject)
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

                    _log.Add("Disconnected[" + Id + "]");

                    var socketNotifier = TcpService as ISocketNotifyer;
                    if (socketNotifier != null)
                        await socketNotifier.Disconnect();

                    (TcpService as IDisposable)?.Dispose();
                }

                _socket.Dispose();
            }
            catch (Exception exception)
            {
                _log.Add("Disconnect error. Id=" + Id + "; Msg:" + exception.Message);
            }

        }

        public async Task SendDataToSocket(object data)
        {
            if (Disconnected)
                return;

            try
            {
                var dataToSocket = _tcpSerializer.Serialize(data);
                var stream = _socket.GetStream();
                await stream.WriteAsync(dataToSocket, 0, dataToSocket.Length);
                _socketStatistic.Sent += dataToSocket.Length;
                _socketStatistic.LastSendTime = DateTime.UtcNow;
                await stream.FlushAsync();
            }
            catch (Exception)
            {
                await Disconnect();
                //_log.Add("SendDataToSocket Error. Id:"+Id+"; Msg:"+ex.Message);
            }
        }

    }
}
