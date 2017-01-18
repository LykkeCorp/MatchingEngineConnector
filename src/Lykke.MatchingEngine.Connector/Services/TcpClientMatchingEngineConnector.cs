using System.Net;
using System.Threading.Tasks;
using Common;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Models;
using Lykke.MatchingEngine.Connector.Tools;

namespace Lykke.MatchingEngine.Connector.Services
{
    public class TcpClientMatchingEngineConnector : IMatchingEngineConnector
    {
        private readonly TasksManager<long, TheResponseModel> _tasksManager = new TasksManager<long, TheResponseModel>();

        private readonly ClientTcpSocket<MatchingEngineSerializer, TcpOrderSocketService> _clientTcpSocket;

        private readonly object _lockObject = new object();
        private long _currentNumber = 1;

        private TcpOrderSocketService _tcpOrderSocketService;

        private long GetNextRequestId()
        {
            lock (_lockObject)
                return _currentNumber++;
        }

        public TcpClientMatchingEngineConnector(IPEndPoint ipEndPoint, ISocketLog socketLog = null)
        {
            _clientTcpSocket = new ClientTcpSocket<MatchingEngineSerializer, TcpOrderSocketService>(
                socketLog,
                ipEndPoint,
                3000,
                () =>
                {
                    _tcpOrderSocketService = new TcpOrderSocketService(_tasksManager);
                    return _tcpOrderSocketService;
                });
        }
        
        public async Task<string> HandleMarketOrderAsync(string clientId, string assetId,
            OrderAction orderAction, double volume, bool straight)
        {
            var id = GetNextRequestId();

            var marketOrderModel = MeMarketOrderModel.Create(id, clientId, assetId, orderAction, volume, straight);
            var resultTask = _tasksManager.Add(id);
            await _tcpOrderSocketService.SendDataToSocket(marketOrderModel);
            var result = await resultTask;

            return result.RecordId;
        }

        public async Task HandleLimitOrderAsync(string clientId, string assetId,
            OrderAction orderAction, double volume, double price)
        {
            var id = GetNextRequestId();

            var limitOrderModel = MeLimitOrderModel.Create(id, clientId, assetId, orderAction, volume, price);
            var resultTask = _tasksManager.Add(id);
            await _tcpOrderSocketService.SendDataToSocket(limitOrderModel);
            await resultTask;
        }

        public async Task TransferAsync(string fromClientId, string toClientId,
            string assetId, double amount, string businessId)
        {
            var id = GetNextRequestId();
            var model = MeTransferModel.Create(id, fromClientId, toClientId, assetId, amount, businessId);
            var resultTask = _tasksManager.Add(id);

            await _tcpOrderSocketService.SendDataToSocket(model);
            await resultTask;
        }

        public async Task<CashInOutResponse> CashInOutBalanceAsync(string clientId, string assetId,
            double balanceDelta, bool sendToBitcoin, string corelationId)
        {
            var id = GetNextRequestId();

            var updateBalanceModel = MeCashInOutModel.Create(id, clientId, assetId, balanceDelta, sendToBitcoin, corelationId);
            var resultTask = _tasksManager.Add(id);
            await _tcpOrderSocketService.SendDataToSocket(updateBalanceModel);
            var result = await resultTask;

            return new CashInOutResponse
            {
                RecordId = result.RecordId,
                CorrelationId = result.CorrelationId
            };
        }

        public async Task UpdateBalanceAsync(string clientId, string assetId, double value)
        {
            var id = GetNextRequestId();
            var model = MeUpdateBalanceModel.Create(id, clientId, assetId, value);

            var resultTask = _tasksManager.Add(id);
            await _tcpOrderSocketService.SendDataToSocket(model);
            await resultTask;
        }

        public async Task CancelLimitOrderAsync(int orderId)
        {
            var id = GetNextRequestId();
            var cancelOrderModel = MeLimitOrderCancelModel.Create(id, orderId);
            var resultTask = _tasksManager.Add(id);
            await _tcpOrderSocketService.SendDataToSocket(cancelOrderModel);
            await resultTask;
        }

        public async Task<bool> UpdateWalletCredsForClient(string clientId)
        {
            var id = GetNextRequestId();
            var updateWalletCredsModel = MeUpdateWalletCredsModel.Create(id, clientId);
            var resultTask = _tasksManager.Add(id);
            await _tcpOrderSocketService.SendDataToSocket(updateWalletCredsModel);
            var result = await resultTask;

            return result.ProcessId == id;
        }

        public void Start()
        {
            _clientTcpSocket.Start();
        }

        public bool IsConnected => _clientTcpSocket.Connected;

        public SocketStatistic SocketStatistic => _clientTcpSocket.SocketStatistic;
    }
}