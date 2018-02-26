using System;
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
        private readonly TasksManager<long, TheResponseModel> _tasksManager =
            new TasksManager<long, TheResponseModel>();
        private readonly TasksManager<string, TheNewResponseModel> _newTasksManager =
            new TasksManager<string, TheNewResponseModel>();
        private readonly TasksManager<string, MarketOrderResponseModel> _marketOrderTasksManager =
            new TasksManager<string, MarketOrderResponseModel>();
        private readonly TasksManager<string, MeMultiLimitOrderResponseModel> _multiLimitOrderTasksManager =
            new TasksManager<string, MeMultiLimitOrderResponseModel>();
        private readonly ClientTcpSocket<MatchingEngineSerializer, TcpOrderSocketService> _clientTcpSocket;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

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
            double value)
        {
            var model = MeNewUpdateBalanceModel.Create(
                id,
                clientId,
                assetId,
                value);
            var resultTask = _newTasksManager.Add(model.Id);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(UpdateBalanceAsync),
                id,
                assetId);
            try
            {
                await _tcpOrderSocketService.SendDataToSocket(model);

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
            double amount)
        {
            var model = MeNewCashInOutModel.Create(
                id,
                clientId,
                assetId,
                amount);

            var resultTask = _newTasksManager.Add(model.Id);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(CashInOutAsync),
                id,
                assetId);
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(model))
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
            FeeSizeType feeSizeType)
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

            var resultTask = _newTasksManager.Add(model.Id);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(CashInOutAsync),
                id,
                $"{assetId} with acc {accuracy} and fee type {feeSizeType}");
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(model))
                {
                    _newTasksManager.Compliete(model.Id, null);
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
            string feeClientId,
            double feeSizePercentage,
            double overdraft)
        {
            var fee = Fee.GenerateTransferFee(
                amount,
                accuracy,
                feeClientId,
                feeSizePercentage);

            var amountWithFee = fee?.Apply(amount) ?? amount;

            var model = MeNewTransferModel.Create(
                id,
                fromClientId,
                toClientId,
                assetId,
                amountWithFee,
                fee?.ToApiModel(),
                overdraft);

            var resultTask = _newTasksManager.Add(model.Id);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(TransferAsync),
                id,
                $"{assetId} with acc {accuracy} and fee size % {feeSizePercentage}");
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(model))
                {
                    _newTasksManager.Compliete(model.Id, null);
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
            double amount2)
        {
            var model = MeNewSwapModel.Create(
                id,
                clientId1,
                assetId1,
                amount1,
                clientId2,
                assetId2,
                amount2);
            var resultTask = _newTasksManager.Add(model.Id);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(SwapAsync),
                id,
                $"From {assetId1} to {assetId2}");
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(model))
                {
                    _newTasksManager.Compliete(model.Id, null);
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

        public async Task<MeResponseModel> PlaceLimitOrderAsync(LimitOrderModel model)
        {
            var limitOrderModel = model.ToNewMeModel();
            var resultTask = _newTasksManager.Add(limitOrderModel.Id);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(PlaceLimitOrderAsync),
                model.Id,
                $"{model.OrderAction} {model.AssetPairId}");
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(limitOrderModel))
                {
                    _newTasksManager.Compliete(model.Id, null);
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

        public async Task<MeResponseModel> CancelLimitOrderAsync(string limitOrderId)
        {
            var model = MeNewLimitOrderCancelModel.Create(Guid.NewGuid().ToString(), limitOrderId);
            var resultTask = _newTasksManager.Add(model.Id);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(CancelLimitOrderAsync),
                limitOrderId,
                null);
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(model))
                {
                    _newTasksManager.Compliete(model.Id, null);
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
            double? reservedLimitVolume = null)
        {
            var id = await GetNextRequestIdAsync();

            var model = MeMarketOrderObsoleteModel.Create(
                id,
                clientId,
                assetPairId,
                orderAction,
                volume,
                straight,
                reservedLimitVolume);
            var resultTask = _tasksManager.Add(model.Id);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(HandleMarketOrderAsync),
                id.ToString(),
                $"{orderAction} {assetPairId}");
            try
            {
                await _tcpOrderSocketService.SendDataToSocket(model);

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

        public async Task<MarketOrderResponse> HandleMarketOrderAsync(MarketOrderModel model)
        {
            var marketOrderModel = model.ToMeModel();

            var resultTask = _marketOrderTasksManager.Add(marketOrderModel.Id);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                "HandleMarketOrderAsync from model",
                model.Id,
                $"{model.OrderAction} {model.AssetPairId}");
            try
            {
                await _tcpOrderSocketService.SendDataToSocket(marketOrderModel);

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

        public async Task<MultiLimitOrderResponse> PlaceMultiLimitOrderAsync(MultiLimitOrderModel model)
        {
            var multiLimitOrderModel = model.ToMeModel();
            var resultTask = _multiLimitOrderTasksManager.Add(model.Id);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(PlaceMultiLimitOrderAsync),
                model.Id,
                model.AssetId);
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(multiLimitOrderModel))
                {
                    _multiLimitOrderTasksManager.Compliete(model.Id, null);
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

        public async Task<MeResponseModel> CancelMultiLimitOrderAsync(MultiLimitOrderCancelModel model)
        {
            var multiLimitOrderCancelModel = model.ToMeModel();
            var resultTask = _newTasksManager.Add(model.Id);

            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                nameof(CancelMultiLimitOrderAsync),
                model.Id,
                $"{model.AssetPairId} IsBuy={model.IsBuy}");
            try
            {
                if (!await _tcpOrderSocketService.SendDataToSocket(multiLimitOrderCancelModel))
                {
                    _newTasksManager.Compliete(model.Id, null);
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

        private async Task<long> GetNextRequestIdAsync()
        {
            await _lock.WaitAsync();
            try
            {
                return _currentNumber++;
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}