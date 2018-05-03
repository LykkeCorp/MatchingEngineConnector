using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Domain;
using Lykke.MatchingEngine.Connector.Models;
using Lykke.MatchingEngine.Connector.Tools;
using Lykke.MatchingEngine.Connector.Helpers;

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

        public async Task UpdateBalanceAsync(
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
            var resultTask = _newTasksManager.Add(model.Id, cancellationToken);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(UpdateBalanceAsync),
                id,
                assetId);
            try
            {
                await _tcpOrderSocketService.SendDataToSocket(model, cancellationToken);

                await resultTask;
            }
            catch (Exception ex)
            {
                TelemetryHelper.SubmitException(ex);
                TelemetryHelper.SubmitOperationFail(telemetryOperation);
            }
            finally
            {
                TelemetryHelper.SubmitOperationResult(telemetryOperation);
            }
        }

        public async Task<MeResponseModel> CashInOutAsync(
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

            var resultTask = _newTasksManager.Add(model.Id, cancellationToken);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(CashInOutAsync),
                id,
                assetId);
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(model, cancellationToken))
                {
                    TelemetryHelper.SubmitOperationFail(telemetryOperation);
                    return null;
                }

                var result = await resultTask;
                return result.ToDomainModel();
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

        public async Task<MeResponseModel> CashInOutAsync(
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
            var fee = Fee.GenerateCashInOutFee(
                amount,
                accuracy,
                feeClientId,
                feeSize,
                feeSizeType);

            var amountWithFee = fee?.Apply(amount) ?? amount;

            var model = MeNewCashInOutModel.Create(
                id,
                clientId,
                assetId,
                amountWithFee,
                fee?.ToApiModel());

            var resultTask = _newTasksManager.Add(model.Id, cancellationToken);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(CashInOutAsync),
                id,
                $"{assetId} with acc {accuracy} and fee type {feeSizeType}");
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(model, cancellationToken))
                {
                    _newTasksManager.SetResult(model.Id, null);
                    TelemetryHelper.SubmitOperationFail(telemetryOperation);
                    return null;
                }

                var result = await resultTask;
                return result.ToDomainModel();
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

        public async Task<MeResponseModel> TransferAsync(
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
            Fee fee = null;
            if (feeModel != null)
            {
                // percentage fee in the system is defined as a share of 1 like 0.025 (it means 2.5%)
                // ME uses percentage fee in percents like 2.5 (the same 2.5%)
                // so it is required to convert value
                double feeSize = feeModel.SizeType == FeeSizeType.PERCENTAGE ?
                    Math.Round(feeModel.Size * 100D, 15).TruncateDecimalPlaces(accuracy, true) :
                    feeModel.Size;

                if (Math.Abs(feeSize) > double.Epsilon)
                {
                    fee = new Fee(feeModel.Type, feeSize, feeModel.SourceClientId, feeModel.TargetClientId, feeModel.SizeType);
                }
            }

            var amountWithFee = fee?.Apply(amount) ?? amount;

            var model = MeNewTransferModel.Create(
                id,
                fromClientId,
                toClientId,
                assetId,
                amountWithFee,
                fee?.ToApiModel(),
                overdraft);

            var resultTask = _newTasksManager.Add(model.Id, cancellationToken);

            string telemetryFeeSiteType = fee == null ? "" : fee.SizeType == FeeSizeType.PERCENTAGE ? " %" : " abs";
            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(TransferAsync),
                id,
                $"{assetId} with acc {accuracy} and fee size{telemetryFeeSiteType} {fee?.Size ?? 0}");
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(model, cancellationToken))
                {
                    _newTasksManager.SetResult(model.Id, null);
                    TelemetryHelper.SubmitOperationFail(telemetryOperation);
                    return null;
                }

                var result = await resultTask;
                return result.ToDomainModel();
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

        public async Task<MeResponseModel> SwapAsync(
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
            var resultTask = _newTasksManager.Add(model.Id, cancellationToken);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(SwapAsync),
                id,
                $"From {assetId1} to {assetId2}");
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(model, cancellationToken))
                {
                    _newTasksManager.SetResult(model.Id, null);
                    TelemetryHelper.SubmitOperationFail(telemetryOperation);
                    return null;
                }

                var result = await resultTask;
                return result.ToDomainModel();
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

        public async Task<MeResponseModel> PlaceLimitOrderAsync(LimitOrderModel model, CancellationToken cancellationToken = default)
        {
            var limitOrderModel = model.ToNewMeModel();
            var resultTask = _newTasksManager.Add(limitOrderModel.Id, cancellationToken);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(PlaceLimitOrderAsync),
                model.Id,
                $"{model.OrderAction} {model.AssetPairId}");
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(limitOrderModel, cancellationToken))
                {
                    _newTasksManager.SetResult(model.Id, null);
                    TelemetryHelper.SubmitOperationFail(telemetryOperation);
                    return null;
                }

                var result = await resultTask;
                return result.ToDomainModel();
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

        public Task<MeResponseModel> CancelLimitOrderAsync(string limitOrderId, CancellationToken cancellationToken = default)
        {
            return CancelLimitOrdersAsync(new[] {limitOrderId}, cancellationToken);
        }

        public async Task<MeResponseModel> CancelLimitOrdersAsync(IEnumerable<string> limitOrderId, CancellationToken cancellationToken = default)
        {
            var idsToCancel = limitOrderId?.ToArray() ?? throw new ArgumentNullException(nameof(limitOrderId));

            var model = MeNewLimitOrderCancelModel.Create(Guid.NewGuid().ToString(), idsToCancel);
            var resultTask = _newTasksManager.Add(model.Id, cancellationToken);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(CancelLimitOrdersAsync),
                idsToCancel.FirstOrDefault(),
                string.Join(", ", idsToCancel));
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(model, cancellationToken))
                {
                    _newTasksManager.SetResult(model.Id, null);
                    TelemetryHelper.SubmitOperationFail(telemetryOperation);
                    return null;
                }

                var result = await resultTask;
                return result.ToDomainModel();
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

        public async Task<string> HandleMarketOrderAsync(
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
            var resultTask = _tasksManager.Add(model.Id, cancellationToken);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(HandleMarketOrderAsync),
                id.ToString(),
                $"{orderAction} {assetPairId}");
            try
            {
                await _tcpOrderSocketService.SendDataToSocket(model, cancellationToken);

                var result = await resultTask;
                return result.RecordId;
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

        public async Task<MarketOrderResponse> HandleMarketOrderAsync(MarketOrderModel model, CancellationToken cancellationToken = default)
        {
            var marketOrderModel = model.ToMeModel();

            var resultTask = _marketOrderTasksManager.Add(marketOrderModel.Id, cancellationToken);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                "HandleMarketOrderAsync from model",
                model.Id,
                $"{model.OrderAction} {model.AssetPairId}");
            try
            {
                await _tcpOrderSocketService.SendDataToSocket(marketOrderModel, cancellationToken);

                var result = await resultTask;
                return new MarketOrderResponse
                {
                    Price = result.Price,
                    Status = (MeStatusCodes)result.Status
                };
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

        public async Task<MultiLimitOrderResponse> PlaceMultiLimitOrderAsync(MultiLimitOrderModel model, CancellationToken cancellationToken = default)
        {
            var multiLimitOrderModel = model.ToMeModel();
            var resultTask = _multiLimitOrderTasksManager.Add(model.Id, cancellationToken);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(PlaceMultiLimitOrderAsync),
                model.Id,
                model.AssetId);
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(multiLimitOrderModel, cancellationToken))
                {
                    _multiLimitOrderTasksManager.SetResult(model.Id, null);
                    TelemetryHelper.SubmitOperationFail(telemetryOperation);
                    return null;
                }

                var result = await resultTask;
                return result.ToDomainModel();
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

        public async Task<MeResponseModel> CancelMultiLimitOrderAsync(MultiLimitOrderCancelModel model, CancellationToken cancellationToken = default)
        {
            var multiLimitOrderCancelModel = model.ToMeModel();
            var resultTask = _newTasksManager.Add(model.Id, cancellationToken);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(CancelMultiLimitOrderAsync),
                model.Id,
                $"{model.AssetPairId} IsBuy={model.IsBuy}");
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(multiLimitOrderCancelModel, cancellationToken))
                {
                    _newTasksManager.SetResult(model.Id, null);
                    TelemetryHelper.SubmitOperationFail(telemetryOperation);
                    return null;
                }

                var result = await resultTask;
                return result.ToDomainModel();
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

        public async Task<MeResponseModel> MassCancelLimitOrdersAsync(LimitOrderMassCancelModel model, CancellationToken cancellationToken = default)
        {
            var limitOrderMassCancelModel = model.ToMeModel();
            var resultTask = _newTasksManager.Add(model.Id, cancellationToken);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(MassCancelLimitOrdersAsync),
                model.Id,
                $"{model.AssetPairId} IsBuy={model.IsBuy}");
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(limitOrderMassCancelModel, cancellationToken))
                {
                    _newTasksManager.SetResult(model.Id, null);
                    TelemetryHelper.SubmitOperationFail(telemetryOperation);
                    return null;
                }

                var result = await resultTask;
                return result.ToDomainModel();
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