using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Lykke.Common.Log;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Helpers;
using Lykke.MatchingEngine.Connector.Models;
using Lykke.MatchingEngine.Connector.Models.Api;
using Lykke.MatchingEngine.Connector.Models.Common;
using Lykke.MatchingEngine.Connector.Models.Me;
using Lykke.MatchingEngine.Connector.Tools;

namespace Lykke.MatchingEngine.Connector.Services
{
    public class TcpMatchingEngineClient : IMatchingEngineClient
    {
        private readonly TimeSpan _defaultReconnectTimeOut = TimeSpan.FromSeconds(3);
        private readonly TasksManager<TheResponseModel> _tasksManager =
            new TasksManager<TheResponseModel>();
        private readonly TasksManager<TheNewResponseModel> _newTasksManager =
            new TasksManager<TheNewResponseModel>();
        private readonly TasksManager<MarketOrderResponseModel> _marketOrderTasksManager =
            new TasksManager<MarketOrderResponseModel>();
        private readonly TasksManager<MeMultiLimitOrderResponseModel> _multiLimitOrderTasksManager =
            new TasksManager<MeMultiLimitOrderResponseModel>();
        private readonly ClientTcpSocket<MatchingEngineSerializer, TcpOrderSocketService> _clientTcpSocket;

        private long _currentNumber = 1;
        private TcpOrderSocketService _tcpOrderSocketService;

        public bool IsConnected => _clientTcpSocket.Connected;

        public SocketStatistic SocketStatistic => _clientTcpSocket.SocketStatistic;

        [Obsolete]
        public TcpMatchingEngineClient(IPEndPoint ipEndPoint, ISocketLog socketLog = null, bool ignoreErrors = false)
        {
            _clientTcpSocket = new ClientTcpSocket<MatchingEngineSerializer, TcpOrderSocketService>(
                socketLog,
                ipEndPoint,
                (int)_defaultReconnectTimeOut.TotalMilliseconds,
                () =>
                {
                    _tcpOrderSocketService = new TcpOrderSocketService(
                        _tasksManager,
                        _newTasksManager,
                        _marketOrderTasksManager,
                        _multiLimitOrderTasksManager,
                        socketLog,
                        ignoreErrors);
                    return _tcpOrderSocketService;
                });
        }

        public TcpMatchingEngineClient(IPEndPoint ipEndPoint, ILogFactory logFactory)
        {
            _clientTcpSocket = new ClientTcpSocket<MatchingEngineSerializer, TcpOrderSocketService>(
                logFactory,
                ipEndPoint,
                (int)_defaultReconnectTimeOut.TotalMilliseconds,
                () =>
                {
                    _tcpOrderSocketService = new TcpOrderSocketService(
                        _tasksManager,
                        _newTasksManager,
                        _marketOrderTasksManager,
                        _multiLimitOrderTasksManager,
                        logFactory);
                    return _tcpOrderSocketService;
                });
        }


        public Task UpdateBalanceAsync(
            string id,
            string clientId,
            string assetId,
            double value,
            CancellationToken cancellationToken = default)
        {
            var model = MeNewUpdateBalanceModel.Create(
                id,
                clientId,
                assetId,
                value);

            return SendData(
                model,
                _newTasksManager,
                x => x.ToDomainModel(),
                cancellationToken,
                id,
                assetId
            );
        }

        public Task<MeResponseModel> UpdateReservedBalanceAsync(
            string id,
            string clientId,
            string assetId,
            double amount,
            CancellationToken cancellationToken = default)
        {
            var model = MeNeweUpdateReservedBalanceModel.Create(
                id,
                clientId,
                assetId,
                amount);
            
            return SendData(
                model,
                _newTasksManager,
                x => x.ToDomainModel(),
                cancellationToken,
                id,
                assetId
            );
        }

        public Task<MeResponseModel> CashInOutAsync(
            string id,
            string clientId,
            string assetId,
            double amount,
            CancellationToken cancellationToken = default)
        {
            var model = MeNewCashInOutModel.Create(
                id,
                clientId,
                assetId,
                amount);

            return SendData(
                model,
                _newTasksManager,
                x => x.ToDomainModel(),
                cancellationToken,
                id,
                assetId
            );
        }

        public Task<MeResponseModel> CashInOutAsync(
            string id,
            string clientId,
            string assetId,
            int accuracy,
            double amount,
            string feeClientId,
            double feeSize,
            FeeSizeType feeSizeType,
            CancellationToken cancellationToken = default)
        {
            var fee = FeeExtensions.GenerateCashInOutFee(
                amount,
                accuracy,
                feeClientId,
                feeSize,
                feeSizeType);

            var amountWithFee = fee?.CalculateAmountWithFee(amount, accuracy) ?? amount.TruncateDecimalPlaces(accuracy, true);

            var model = MeNewCashInOutModel.Create(
                id,
                clientId,
                assetId,
                amountWithFee,
                fee?.ToMeModel());

            return SendData(
                model,
                _newTasksManager,
                x => x.ToDomainModel(),
                cancellationToken,
                id,
                $"{assetId} with acc {accuracy} and fee type {feeSizeType}"
            );
        }

        public Task<MeResponseModel> TransferAsync(
            string id,
            string fromClientId,
            string toClientId,
            string assetId,
            int accuracy,
            double amount,
            FeeModel feeModel,
            double overdraft,
            CancellationToken cancellationToken = default)
        {
            var amountToTransfer = amount.TruncateDecimalPlaces(accuracy, true);

            FeeModel fee = null;
            if (feeModel != null)
            {
                if (feeModel.SizeType == FeeSizeType.PERCENTAGE) // create absolute fee from percentage
                {
                    fee = feeModel.GenerateTransferFee(amount, accuracy);
                }
                else
                {
                    fee = feeModel;
                }

                switch (feeModel.ChargingType)
                {
                    case FeeChargingType.RAISE_AMOUNT:
                        amountToTransfer = fee?.CalculateAmountWithFee(amount, accuracy) ?? amountToTransfer;
                        break;
                    case FeeChargingType.SUBTRACT_FROM_AMOUNT:
                        // default ME behavior - no any amoung change required
                        break;
                    default:
                        if (feeModel.Type != FeeType.EXTERNAL_FEE)
                        {
                            throw new ArgumentOutOfRangeException(nameof(feeModel.ChargingType));
                        }
                        break;
                }
            }

            var model = MeNewTransferModel.Create(
                id,
                fromClientId,
                toClientId,
                assetId,
                amountToTransfer,
                fee?.ToMeModel(),
                overdraft);
            string telemetryFeeSiteType = fee == null ? "" : fee.SizeType == FeeSizeType.PERCENTAGE ? " %" : " abs";

            return SendData(
                model,
                _newTasksManager,
                x => x.ToDomainModel(),
                cancellationToken,
                id,
                $"{assetId} with acc {accuracy} and fee size{telemetryFeeSiteType} {fee?.Size ?? 0}"
            );
        }

        public Task<MeResponseModel> SwapAsync(
            string id,
            string clientId1,
            string assetId1,
            double amount1,
            string clientId2,
            string assetId2,
            double amount2,
            CancellationToken cancellationToken = default)
        {
            var model = MeNewSwapModel.Create(
                id,
                clientId1,
                assetId1,
                amount1,
                clientId2,
                assetId2,
                amount2);

            return SendData(
                model,
                _newTasksManager,
                x => x.ToDomainModel(),
                cancellationToken,
                model.Id,
                $"From {assetId1} to {assetId2}"
            );
        }

        public Task<MeResponseModel> PlaceLimitOrderAsync(LimitOrderModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return SendData(
                model.ToNewMeModel(),
                _newTasksManager,
                x => x.ToDomainModel(),
                cancellationToken,
                model.Id,
                $"{model.OrderAction} {model.AssetPairId}"
            );
        }

        public Task<MeResponseModel> PlaceStopLimitOrderAsync(StopLimitOrderModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return SendData(
                model.ToNewMeModel(),
                _newTasksManager,
                x => x.ToDomainModel(),
                cancellationToken,
                model.Id,
                $"{model.OrderAction} {model.AssetPairId}"
            );
        }

        public Task<MeResponseModel> CancelLimitOrderAsync(string limitOrderId, CancellationToken cancellationToken = default)
        {
            return CancelLimitOrdersAsync(new[] { limitOrderId }, cancellationToken);
        }

        public Task<MeResponseModel> CancelLimitOrdersAsync(IEnumerable<string> limitOrderId, CancellationToken cancellationToken = default)
        {
            var idsToCancel = limitOrderId?.ToArray() ?? throw new ArgumentNullException(nameof(limitOrderId));
            var model = MeNewLimitOrderCancelModel.Create(Guid.NewGuid().ToString(), idsToCancel);

            return SendData(
                model,
                _newTasksManager,
                x => x.ToDomainModel(),
                cancellationToken,
                model.Id,
                string.Join(", ", idsToCancel)
            );
        }

        public Task<string> HandleMarketOrderAsync(
            string clientId,
            string assetPairId,
            OrderAction orderAction,
            double volume,
            bool straight,
            double? reservedLimitVolume = null,
            CancellationToken cancellationToken = default)
        {

            var id = GetNextRequestId();
            var model = MeMarketOrderObsoleteModel.Create(
                id,
                clientId,
                assetPairId,
                orderAction,
                volume,
                straight,
                reservedLimitVolume);

            return SendData(
                model,
                _tasksManager,
                x => x.RecordId,
                cancellationToken,
                model.Id.ToString(),
                $"{orderAction} {assetPairId}"
            );
        }

        public Task<MarketOrderResponse> HandleMarketOrderAsync(MarketOrderModel model, CancellationToken cancellationToken = default)
        {
            return SendData(
                model.ToMeModel(),
                _marketOrderTasksManager,
                x => new MarketOrderResponse
                {
                    Price = x.Price,
                    Status = (MeStatusCodes)x.Status
                },
                cancellationToken,
                model.Id,
                $"{model.OrderAction} {model.AssetPairId}"
            );
        }

        public Task<MultiLimitOrderResponse> PlaceMultiLimitOrderAsync(MultiLimitOrderModel model, CancellationToken cancellationToken = default)
        {
            return SendData(
                model.ToMeModel(),
                _multiLimitOrderTasksManager,
                x => x.ToDomainModel(),
                cancellationToken,
                model.Id,
                model.AssetPairId
            );
        }

        public Task<MeResponseModel> CancelMultiLimitOrderAsync(MultiLimitOrderCancelModel model, CancellationToken cancellationToken = default)
        {
            return SendData(
                model.ToMeModel(),
                _newTasksManager,
                x => x.ToDomainModel(),
                cancellationToken,
                model.Id,
                $"{model.AssetPairId} IsBuy={model.IsBuy}"
            );
        }

        public Task<MeResponseModel> MassCancelLimitOrdersAsync(LimitOrderMassCancelModel model, CancellationToken cancellationToken = default)
        {
            return SendData(
                model.ToMeModel(),
                _newTasksManager,
                x => x.ToDomainModel(),
                cancellationToken,
                model.Id,
                $"{model.AssetPairId} IsBuy={model.IsBuy}"
            );
        }

        private async Task<TResponse> SendData<TModel, TResult, TResponse>(
            TModel model,
            TasksManager<TResult> manager,
            Func<TResult, TResponse> convert,
            CancellationToken cancellationToken,
            string id,
            string telemetryData,
            [CallerMemberName] string callerName = "")
            where TResult : class
            where TResponse : class
        {
            var resultTask = manager.Add(id, cancellationToken);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                callerName,
                id,
                telemetryData);
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(model, cancellationToken))
                {
                    manager.SetResult(id, null);
                    TelemetryHelper.SubmitOperationFail(telemetryOperation);
                    return null;
                }

                var result = await resultTask;
                return convert(result);
            }
            catch (Exception ex)
            {
                TelemetryHelper.SubmitException(ex);
                TelemetryHelper.SubmitOperationFail(telemetryOperation);
                throw;
            }
            finally
            {
                TelemetryHelper.SubmitOperationResult(telemetryOperation);
            }
        }

        public void Start()
        {
            _clientTcpSocket.Start();
        }

        private long GetNextRequestId()
        {
            return Interlocked.Increment(ref _currentNumber);
        }
    }
}
