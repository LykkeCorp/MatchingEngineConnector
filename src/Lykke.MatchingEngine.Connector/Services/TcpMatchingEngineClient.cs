using System.Net;
using System.Threading.Tasks;
using Common;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Models;
using Lykke.MatchingEngine.Connector.Tools;

namespace Lykke.MatchingEngine.Connector.Services
{
    public class TcpMatchingEngineClient : IMatchingEngineClient
    {
        private readonly TasksManager<long, TheResponseModel> _tasksManager =
            new TasksManager<long, TheResponseModel>();

        private readonly TasksManager<string, TheNewResponseModel> _newTasksManager = 
            new TasksManager<string, TheNewResponseModel>();

        private readonly ClientTcpSocket<MatchingEngineSerializer, TcpOrderSocketService> _clientTcpSocket;

        private readonly object _lockObject = new object();
        private long _currentNumber = 1;

        private TcpOrderSocketService _tcpOrderSocketService;

        private long GetNextRequestId()
        {
            lock (_lockObject)
                return _currentNumber++;
        }

        public TcpMatchingEngineClient(IPEndPoint ipEndPoint, ISocketLog socketLog = null)
        {
            _clientTcpSocket = new ClientTcpSocket<MatchingEngineSerializer, TcpOrderSocketService>(
                socketLog,
                ipEndPoint,
                3000,
                () =>
                {
                    _tcpOrderSocketService = new TcpOrderSocketService(_tasksManager, _newTasksManager);
                    return _tcpOrderSocketService;
                });
        }

        public async Task UpdateBalanceAsync(string clientId, string assetId, double value)
        {
            var id = GetNextRequestId();
            var model = MeUpdateBalanceModel.Create(id, clientId, assetId, value);

            var resultTask = _tasksManager.Add(id);
            await _tcpOrderSocketService.SendDataToSocket(model);
            await resultTask;
        }

        public async Task<MeResponseModel> CashInOutAsync(string id, string clientId, string assetId, double amount)
        {
            var model = MeNewCashInOutModel.Create(id, clientId, assetId, amount);
            var resultTask = _newTasksManager.Add(id);

            if (!await _tcpOrderSocketService.SendDataToSocket(model))
                return null;

            var result = await resultTask;
            return result.ToDomainModel();
        }

        public async Task<MeResponseModel> TransferAsync(string id, string fromClientId,
            string toClientId, string assetId, double amount)
        {
            var model = MeNewTransferModel.Create(id, fromClientId, toClientId, assetId, amount);
            var resultTask = _newTasksManager.Add(id);

            if (!await _tcpOrderSocketService.SendDataToSocket(model))
                return null;

            var result = await resultTask;
            return result.ToDomainModel();
        }

        public async Task<MeResponseModel> SwapAsync(string id,
            string clientId1, string assetId1, double amount1,
            string clientId2, string assetId2, double amount2)
        {
            var model = MeNewSwapModel.Create(id,
                clientId1, assetId1, amount1,
                clientId2, assetId2, amount2);
            var resultTask = _newTasksManager.Add(id);

            if (!await _tcpOrderSocketService.SendDataToSocket(model))
                return null;

            var result = await resultTask;
            return result.ToDomainModel();
        }

        public async Task<MeResponseModel> PlaceLimitOrderAsync(string id,
            string clientId, string assetPairId,
            OrderAction orderAction, double volume, double price)
        {
            var model = MeNewLimitOrderModel.Create(id,
                clientId, assetPairId, orderAction, volume, price);
            var resultTask = _newTasksManager.Add(id);

            if (!await _tcpOrderSocketService.SendDataToSocket(model))
                return null;

            var result = await resultTask;
            return result.ToDomainModel();
        }

        public async Task<MeResponseModel> CancelLimitOrderAsync(string id, string limitOrderId)
        {
            var model = MeNewLimitOrderCancelModel.Create(id, limitOrderId);
            var resultTask = _newTasksManager.Add(id);

            if (!await _tcpOrderSocketService.SendDataToSocket(model))
                return null;

            var result = await resultTask;
            return result.ToDomainModel();
        }

        public void Start()
        {
            _clientTcpSocket.Start();
        }

        public bool IsConnected => _clientTcpSocket.Connected;

        public SocketStatistic SocketStatistic => _clientTcpSocket.SocketStatistic;
    }
}