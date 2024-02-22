using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Models.Me;
using Microsoft.Extensions.Logging;

namespace Lykke.MatchingEngine.Connector.Services
{
    internal class TcpOrderSocketService : ITcpClientService, ISocketNotifier
    {
        private readonly TasksManager<TheResponseModel> _tasksManager;
        private readonly TasksManager<TheNewResponseModel> _newTasksManager;
        private readonly TasksManager<MarketOrderResponseModel> _marketOrdersTasksManager;
        private readonly TasksManager<MeMultiLimitOrderResponseModel> _multiOrdersTasksManager;
        private readonly bool _ignoreErrors;
        private readonly bool _logResponse;
        private readonly ILogger<TcpOrderSocketService> _logger;
        

        public TcpOrderSocketService(TasksManager<TheResponseModel> tasksManager,
            TasksManager<TheNewResponseModel> newTasksManager,
            TasksManager<MarketOrderResponseModel> marketOrdersTasksManager,
            TasksManager<MeMultiLimitOrderResponseModel> multiOrdersTasksManager,
            ILogger<TcpOrderSocketService> logger,
            bool ignoreErrors = false,
            bool logResponse = true)
        {
            _tasksManager = tasksManager;
            _newTasksManager = newTasksManager;
            _marketOrdersTasksManager = marketOrdersTasksManager;
            _multiOrdersTasksManager = multiOrdersTasksManager;
            _logger = logger;
            _ignoreErrors = ignoreErrors;
            _logResponse = logResponse;
        }

        public void HandleDataFromSocket(object data)
        {
            try
            {
                switch (data)
                {
                    case TheResponseModel theResponse:
                        if (_logResponse)
                            _logger.LogInformation("Response: {@MeResponse)}", theResponse);
                        
                        _tasksManager.SetResult(theResponse.ProcessId, theResponse);
                        break;

                    case TheNewResponseModel theNewResponse:
                        if (_logResponse)
                            _logger.LogInformation("Response: {@MeResponse)}", theNewResponse);
                        
                        _newTasksManager.SetResult(theNewResponse.Id, theNewResponse);
                        break;

                    case MarketOrderResponseModel theMarketOrderResponse:
                        if (_logResponse)
                            _logger.LogInformation("Response: {@MeResponse)}", theMarketOrderResponse);
                        
                        _marketOrdersTasksManager.SetResult(theMarketOrderResponse.Id, theMarketOrderResponse);
                        break;

                    case MeMultiLimitOrderResponseModel multiLimitOrderResponse:
                        if (_logResponse)
                            _logger.LogInformation("Response: {@MeResponse)}", multiLimitOrderResponse);
                        
                        _multiOrdersTasksManager.SetResult(multiLimitOrderResponse.Id, multiLimitOrderResponse);
                        break;

                    // No handlers for the following messages
                    case MePingModel:
                    case MeCashInOutModel:
                    case MeLimitOrderModel:
                    case MeMarketOrderObsoleteModel:
                    case MeLimitOrderCancelModel:
                    case MeUpdateBalanceModel:
                    case MeNewTransferModel:
                    case MeNewCashInOutModel:
                    case MeNewSwapModel:
                    case MeUpdateWalletCredsModel:
                    case MeNewLimitOrderModel:
                    case MeNewLimitOrderCancelModel:
                    case MeNewMarketOrderModel:
                    case MeMarketOrderModel:
                    case MeNewUpdateBalanceModel:
                    case MeMultiLimitOrderModel:
                    case MeMultiLimitOrderCancelModel:
                    case MeNewUpdateReservedBalanceModel:
                        break;
                    default:
                        throw new ArgumentException(nameof(data), $"{data.GetType().Name} is not mapped. Please check the mapping in the MatchingEngineSerializer class");
                }
            }
            catch (KeyNotFoundException exception)
            {
                _logger?.LogError(exception, "Error processing data {@MeResponse}", data);

                if (!_ignoreErrors)
                    throw;
            }
        }

        public Func<object, CancellationToken, Task<bool>> SendDataToSocket { get; set; }
        public string ContextName => "TcpSocket";
        public object GetPingData()
        {
            return MePingModel.Instance;
        }

        public Task Connect()
        {
            return Task.CompletedTask;
        }

        public Task Disconnect(Exception exc)
        {
            exc ??= new Exception("Socket disconnected");

            _tasksManager.SetExceptionsToAll(exc);
            _newTasksManager.SetExceptionsToAll(exc);
            _marketOrdersTasksManager.SetExceptionsToAll(exc);
            _multiOrdersTasksManager.SetExceptionsToAll(exc);

            return Task.CompletedTask;
        }
    }
}
