using System;
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
        private readonly TasksManager<long, TheResponseModel> _tasksManager =
            new TasksManager<long, TheResponseModel>();

        private readonly TasksManager<string, MarketOrderResponseModel> _marketOrderTasksManager =
            new TasksManager<string, MarketOrderResponseModel>();

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

        public TcpClientMatchingEngineConnector(IPEndPoint ipEndPoint, ISocketLog socketLog = null, bool ignoreErrors = false)
        {
            _clientTcpSocket = new ClientTcpSocket<MatchingEngineSerializer, TcpOrderSocketService>(
                socketLog,
                ipEndPoint,
                3000,
                () =>
                {
                    _tcpOrderSocketService = new TcpOrderSocketService(_tasksManager, _newTasksManager,
                        _marketOrderTasksManager, ignoreErrors);
                    return _tcpOrderSocketService;
                });
        }
        
        public async Task<string> HandleMarketOrderObsoleteAsync(string clientId, string assetId, OrderAction orderAction, double volume, bool straight, double? reservedLimitVolume = null)
        {
            var id = GetNextRequestId();

            var marketOrderModel = MeMarketOrderObsoleteModel.Create(id, clientId, assetId, orderAction, volume, straight, reservedLimitVolume);
            var resultTask = _tasksManager.Add(id);
            await _tcpOrderSocketService.SendDataToSocket(marketOrderModel);
            var result = await resultTask;

            return result.RecordId;
        }

        public async Task<string> HandleMarketOrderObsoleteAsync(string id, string clientId, string assetPairId, OrderAction orderAction, double volume, bool straight, double? reservedLimitVolume = null)
        {
            var marketOrderModel = MeNewMarketOrderModel.Create(id, clientId, assetPairId, orderAction, volume, straight, reservedLimitVolume);
            var resultTask = _newTasksManager.Add(id);
            await _tcpOrderSocketService.SendDataToSocket(marketOrderModel);
            var result = await resultTask;

            return result.MatchingEngineId;
        }

        public async Task<MarketOrderResponse> HandleMarketOrderAsync(MarketOrderModel model)
        {
            var marketOrderModel = model.ToMeModel();
            var resultTask = _marketOrderTasksManager.Add(marketOrderModel.Id);
            await _tcpOrderSocketService.SendDataToSocket(marketOrderModel);
            var result = await resultTask;

            return new MarketOrderResponse
            {
                Price = result.Price,
                Status = (MeStatusCodes) result.Status
            };
        }

        public async Task<MeResponseModel> HandleLimitOrderAsync(LimitOrderModel model)
        {
            var limitOrderModel = model.ToNewMeModel();
            var resultTask = _newTasksManager.Add(limitOrderModel.Id);

            if (!await _tcpOrderSocketService.SendDataToSocket(limitOrderModel))
                return null;

            var result = await resultTask;
            return result.ToDomainModel();
        }

        public async Task<MeResponseModel> CancelLimitOrderAsync(string limitOrderId)
        {
            var model = MeNewLimitOrderCancelModel.Create(Guid.NewGuid().ToString(), limitOrderId);
            var resultTask = _newTasksManager.Add(model.Id);

            if (!await _tcpOrderSocketService.SendDataToSocket(model))
                return null;

            var result = await resultTask;
            return result.ToDomainModel();
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

        public async Task UpdateBalanceAsync(string id, string clientId, string assetId, double value)
        {
            var model = MeNewUpdateBalanceModel.Create(id, clientId, assetId, value);

            var resultTask = _newTasksManager.Add(id);
            await _tcpOrderSocketService.SendDataToSocket(model);
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
            string toClientId, string assetId, double amount, string feeClientId, double feeSizePercentage, double overdraft)
        {
            var model = MeNewTransferModel.Create(id, fromClientId, toClientId, assetId, amount, feeClientId, feeSizePercentage, overdraft);
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

        public void Start()
        {
            _clientTcpSocket.Start();
        }

        public bool IsConnected => _clientTcpSocket.Connected;

        public SocketStatistic SocketStatistic => _clientTcpSocket.SocketStatistic;
    }
}