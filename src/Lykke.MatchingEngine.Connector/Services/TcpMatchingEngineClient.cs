using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Helpers;
using Lykke.MatchingEngine.Connector.Models;
using Lykke.MatchingEngine.Connector.Models.Api;
using Lykke.MatchingEngine.Connector.Models.Common;
using Lykke.MatchingEngine.Connector.Models.Me;
using Lykke.MatchingEngine.Connector.Tools;
using Polly;
using Polly.Retry;

namespace Lykke.MatchingEngine.Connector.Services
{
    ///<inheritdoc cref="IMatchingEngineClient"/>
    public class TcpMatchingEngineClient : IMatchingEngineClient
    {
        private readonly bool _enableRetries;
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

        /// <inheritdoc />
        public bool IsConnected => _clientTcpSocket.Connected;

        ///<inheritdoc cref="IMatchingEngineClient"/>
        public SocketStatistic SocketStatistic => _clientTcpSocket.SocketStatistic;

        private AsyncRetryPolicy<MeResponseModel> _meResponsePolicy;
        private AsyncRetryPolicy<MarketOrderResponse> _marketOrderResponsePolicy;
        private AsyncRetryPolicy<MultiLimitOrderResponse> _multiLimitOrderResponsePolicy;
        private AsyncRetryPolicy<string> _marketOrderOldResponsePolicy;

        ///<inheritdoc cref="IMatchingEngineClient"/>
        public TcpMatchingEngineClient(
            IPEndPoint ipEndPoint,
            ILogFactory logFactory,
            bool enableRetries,
            bool ignoreErrors = false)
        {
            _enableRetries = enableRetries;
            CreatePolicies(logFactory.CreateLog(this));
            _clientTcpSocket = new ClientTcpSocket<MatchingEngineSerializer, TcpOrderSocketService>(
                logFactory,
                ipEndPoint,
                (int) _defaultReconnectTimeOut.TotalMilliseconds,
                () =>
                {
                    _tcpOrderSocketService = new TcpOrderSocketService(
                        _tasksManager,
                        _newTasksManager,
                        _marketOrderTasksManager,
                        _multiLimitOrderTasksManager,
                        logFactory,
                        ignoreErrors);
                    return _tcpOrderSocketService;
                });
        }

        ///<inheritdoc cref="IMatchingEngineClient"/>
        public TcpMatchingEngineClient(
            MeClientSettings settings,
            ILogFactory logFactory)
        {
            CreatePolicies(logFactory.CreateLog(this));
            _enableRetries = settings.EnableRetries;
            _clientTcpSocket = new ClientTcpSocket<MatchingEngineSerializer, TcpOrderSocketService>(
                logFactory,
                settings,
                () =>
                {
                    _tcpOrderSocketService = new TcpOrderSocketService(
                        _tasksManager,
                        _newTasksManager,
                        _marketOrderTasksManager,
                        _multiLimitOrderTasksManager,
                        logFactory,
                        settings.IgnoreErrors,
                        settings.LogResponse);
                    return _tcpOrderSocketService;
                });
        }

        ///<inheritdoc cref="IMatchingEngineClient"/>
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
                _meResponsePolicy,
                cancellationToken,
                id,
                assetId
            );
        }

        ///<inheritdoc cref="IMatchingEngineClient"/>
        public Task<MeResponseModel> UpdateReservedBalanceAsync(
            string id,
            string clientId,
            string assetId,
            double amount,
            CancellationToken cancellationToken = default)
        {
            var model = MeNewUpdateReservedBalanceModel.Create(
                id,
                clientId,
                assetId,
                amount);

            return SendData(
                model,
                _newTasksManager,
                x => x.ToDomainModel(),
                _meResponsePolicy,
                cancellationToken,
                id,
                assetId
            );
        }

        ///<inheritdoc cref="IMatchingEngineClient"/>
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
                _meResponsePolicy,
                cancellationToken,
                id,
                assetId
            );
        }

        ///<inheritdoc cref="IMatchingEngineClient"/>
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

            var amountWithFee = fee?.CalculateAmountWithFee(amount, accuracy) ??
                                amount.TruncateDecimalPlaces(accuracy, true);

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
                _meResponsePolicy,
                cancellationToken,
                id,
                $"{assetId} with acc {accuracy} and fee type {feeSizeType}"
            );
        }

        ///<inheritdoc cref="IMatchingEngineClient"/>
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
                fee = feeModel.SizeType == FeeSizeType.PERCENTAGE // create absolute fee from percentage
                    ? feeModel.GenerateTransferFee(amount, accuracy)
                    : feeModel;

                switch (feeModel.ChargingType)
                {
                    case FeeChargingType.RAISE_AMOUNT:
                        amountToTransfer = fee?.CalculateAmountWithFee(amount, accuracy) ?? amountToTransfer;
                        break;
                    case FeeChargingType.SUBTRACT_FROM_AMOUNT:
                        // default ME behavior - no any among change required
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
                _meResponsePolicy,
                cancellationToken,
                id,
                $"{assetId} with acc {accuracy} and fee size{telemetryFeeSiteType} {fee?.Size ?? 0}"
            );
        }

        ///<inheritdoc cref="IMatchingEngineClient"/>
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
                _meResponsePolicy,
                cancellationToken,
                model.Id,
                $"From {assetId1} to {assetId2}"
            );
        }

        ///<inheritdoc cref="IMatchingEngineClient"/>
        public Task<MeResponseModel> PlaceLimitOrderAsync(LimitOrderModel model,
            CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return SendData(
                model.ToNewMeModel(),
                _newTasksManager,
                x => x.ToDomainModel(),
                _meResponsePolicy,
                cancellationToken,
                model.Id,
                $"{model.OrderAction} {model.AssetPairId}"
            );
        }

        ///<inheritdoc cref="IMatchingEngineClient"/>
        public Task<MeResponseModel> PlaceStopLimitOrderAsync(StopLimitOrderModel model,
            CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return SendData(
                model.ToNewMeModel(),
                _newTasksManager,
                x => x.ToDomainModel(),
                _meResponsePolicy,
                cancellationToken,
                model.Id,
                $"{model.OrderAction} {model.AssetPairId}"
            );
        }

        ///<inheritdoc cref="IMatchingEngineClient"/>
        public Task<MeResponseModel> CancelLimitOrderAsync(string limitOrderId,
            CancellationToken cancellationToken = default)
        {
            return CancelLimitOrdersAsync(new[] {limitOrderId}, cancellationToken);
        }

        ///<inheritdoc cref="IMatchingEngineClient"/>
        public Task<MeResponseModel> CancelLimitOrdersAsync(IEnumerable<string> limitOrderId,
            CancellationToken cancellationToken = default)
        {
            var idsToCancel = limitOrderId?.ToArray() ?? throw new ArgumentNullException(nameof(limitOrderId));
            var model = MeNewLimitOrderCancelModel.Create(Guid.NewGuid().ToString(), idsToCancel);

            return SendData(
                model,
                _newTasksManager,
                x => x.ToDomainModel(),
                _meResponsePolicy,
                cancellationToken,
                model.Id,
                string.Join(", ", idsToCancel)
            );
        }

        ///<inheritdoc cref="IMatchingEngineClient"/>
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
                _marketOrderOldResponsePolicy,
                cancellationToken,
                model.Id.ToString(),
                $"{orderAction} {assetPairId}"
            );
        }

        ///<inheritdoc cref="IMatchingEngineClient"/>
        public Task<MarketOrderResponse> HandleMarketOrderAsync(MarketOrderModel model,
            CancellationToken cancellationToken = default)
        {
            return SendData(
                model.ToMeModel(),
                _marketOrderTasksManager,
                x => new MarketOrderResponse
                {
                    Price = x.Price,
                    Status = (MeStatusCodes) x.Status
                },
                _marketOrderResponsePolicy,
                cancellationToken,
                model.Id,
                $"{model.OrderAction} {model.AssetPairId}"
            );
        }

        ///<inheritdoc cref="IMatchingEngineClient"/>
        public Task<MultiLimitOrderResponse> PlaceMultiLimitOrderAsync(MultiLimitOrderModel model,
            CancellationToken cancellationToken = default)
        {
            return SendData(
                model.ToMeModel(),
                _multiLimitOrderTasksManager,
                x => x.ToDomainModel(),
                _multiLimitOrderResponsePolicy,
                cancellationToken,
                model.Id,
                model.AssetPairId
            );
        }

        ///<inheritdoc cref="IMatchingEngineClient"/>
        public Task<MeResponseModel> CancelMultiLimitOrderAsync(MultiLimitOrderCancelModel model,
            CancellationToken cancellationToken = default)
        {
            return SendData(
                model.ToMeModel(),
                _newTasksManager,
                x => x.ToDomainModel(),
                _meResponsePolicy,
                cancellationToken,
                model.Id,
                $"{model.AssetPairId} IsBuy={model.IsBuy}"
            );
        }

        ///<inheritdoc cref="IMatchingEngineClient"/>
        public Task<MeResponseModel> MassCancelLimitOrdersAsync(LimitOrderMassCancelModel model,
            CancellationToken cancellationToken = default)
        {
            return SendData(
                model.ToMeModel(),
                _newTasksManager,
                x => x.ToDomainModel(),
                _meResponsePolicy,
                cancellationToken,
                model.Id,
                $"{model.AssetPairId} IsBuy={model.IsBuy}"
            );
        }

        private async Task<TResponse> SendData<TModel, TResult, TResponse>(
            TModel model,
            TasksManager<TResult> manager,
            Func<TResult, TResponse> convert,
            AsyncRetryPolicy<TResponse> retryPolicy,
            CancellationToken cancellationToken,
            string id,
            string telemetryData,
            [CallerMemberName] string callerName = "")
            where TResult : class
            where TResponse : class
        {
            var telemetryOperation = TelemetryHelper.InitTelemetryOperation(
                callerName,
                id,
                telemetryData);

            try
            {
                if (_enableRetries)
                {
                    var response = await retryPolicy.ExecuteAsync(async () =>
                    {
                        var resultTask = manager.Add(id, cancellationToken);

                        if (!await _tcpOrderSocketService.SendDataToSocket(model, cancellationToken))
                        {
                            manager.SetResult(id, null);
                            TelemetryHelper.SubmitOperationFail(telemetryOperation);
                            return null;
                        }

                        var result = await resultTask;
                        return convert(result);
                    });

                    return response;
                }
                else
                {
                    var resultTask = manager.Add(id, cancellationToken);

                    if (!await _tcpOrderSocketService.SendDataToSocket(model, cancellationToken))
                    {
                        manager.SetResult(id, null);
                        TelemetryHelper.SubmitOperationFail(telemetryOperation);
                        return null;
                    }

                    var result = await resultTask;
                    return convert(result);
                }
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

        ///<inheritdoc cref="IMatchingEngineClient"/>
        public void Start()
        {
            _clientTcpSocket.Start();
        }

        private long GetNextRequestId()
        {
            return Interlocked.Increment(ref _currentNumber);
        }

        private void CreatePolicies(ILog log)
        {
            _meResponsePolicy = Policy
                .Handle<Exception>(exception =>
                {
                    log.Warning("Retry on exception", exception);
                    return true;
                })
                .OrResult<MeResponseModel>(r =>
                {
                    if (r == null)
                    {
                        log.Warning("Retry on null response from ME");
                        return true;
                    }

                    if (r.Status == MeStatusCodes.Runtime)
                    {
                        log.Warning("Retry on runtime error from ME");
                        return true;
                    }

                    return false;
                }) //ME not available or runtime error
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            _marketOrderResponsePolicy = Policy
                .Handle<Exception>(exception =>
                {
                    log.Warning("Retry on exception", exception);
                    return true;
                })
                .OrResult<MarketOrderResponse>(r =>
                {
                    if (r == null)
                    {
                        log.Warning("Retry on null response from ME");
                        return true;
                    }

                    if (r.Status == MeStatusCodes.Runtime)
                    {
                        log.Warning("Retry on runtime error from ME");
                        return true;
                    }

                    return false;
                }) //ME not available or runtime error
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            _multiLimitOrderResponsePolicy = Policy
                .Handle<Exception>(exception =>
                {
                    log.Warning("Retry on exception", exception);
                    return true;
                })
                .OrResult<MultiLimitOrderResponse>(r =>
                {
                    if (r == null)
                    {
                        log.Warning("Retry on null response from ME");
                        return true;
                    }

                    if (r.Status == MeStatusCodes.Runtime)
                    {
                        log.Warning("Retry on runtime error from ME");
                        return true;
                    }

                    return false;
                }) //ME not available or runtime error
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            _marketOrderOldResponsePolicy = Policy
                .Handle<Exception>(exception =>
                {
                    log.Warning("Retry on exception", exception);
                    return true;
                })
                .OrResult<string>(r =>
                {
                    if (r == null)
                    {
                        log.Warning("Retry on null response from ME");
                        return true;
                    }

                    return false;
                }) //ME not available or runtime error
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
