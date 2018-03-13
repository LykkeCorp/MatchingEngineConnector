using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Models;

namespace Lykke.MatchingEngine.Connector.Services
{
    internal class TcpOrderSocketService : ITcpClientService, ISocketNotifier
    {
        private readonly TasksManager<TheResponseModel> _tasksManager;
        private readonly TasksManager<TheNewResponseModel> _newTasksManager;
        private readonly TasksManager<MarketOrderResponseModel> _marketOrdersTasksManager;
        private readonly TasksManager<MeMultiLimitOrderResponseModel> _multiOrdersTasksManager;
        private readonly ISocketLog _logger;
        private readonly bool _ignoreErrors;

        public TcpOrderSocketService(TasksManager<TheResponseModel> tasksManager,
            TasksManager<TheNewResponseModel> newTasksManager,
            TasksManager<MarketOrderResponseModel> marketOrdersTasksManager,
            TasksManager<MeMultiLimitOrderResponseModel> multiOrdersTasksManager,
            ISocketLog logger = null,
            bool ignoreErrors = false)
        {
            _tasksManager = tasksManager;
            _newTasksManager = newTasksManager;
            _marketOrdersTasksManager = marketOrdersTasksManager;
            _multiOrdersTasksManager = multiOrdersTasksManager;
            _logger = logger;
            _ignoreErrors = ignoreErrors;
        }

        public void HandleDataFromSocket(object data)
        {
            try
            {
                switch (data)
                {
                    case TheResponseModel theResponse:
                        _logger?.Add($"Response ProcessId: {theResponse.ProcessId}");
                        _tasksManager.SetResult(theResponse.ProcessId, theResponse);
                        break;
                    case TheNewResponseModel theNewResponse:
                        _logger?.Add($"Response Id: {theNewResponse.Id}");
                        _newTasksManager.SetResult(theNewResponse.Id, theNewResponse);
                        break;
                    case MarketOrderResponseModel theMarketOrderResponse:
                        _logger?.Add($"Response Id: {theMarketOrderResponse.Id}");
                        _marketOrdersTasksManager.SetResult(theMarketOrderResponse.Id, theMarketOrderResponse);
                        break;
                    case MeMultiLimitOrderResponseModel multiLimitOrderResponse:
                        _logger?.Add($"Response Id: {multiLimitOrderResponse.Id}");
                        _multiOrdersTasksManager.SetResult(multiLimitOrderResponse.Id, multiLimitOrderResponse);
                        break;
                    // No handlers for the following messages
                    case MePingModel m0: 
                    case MeCashInOutModel m1:
                    case MeLimitOrderModel m2:
                    case MeLimitOrderCancelModel m3:
                    case MeUpdateBalanceModel m4:
                    case MeUpdateWalletCredsModel m5:
                    case MeNewMarketOrderModel m6:
                        break;
                    default:
                        throw new ArgumentException(nameof(data), $"{data.GetType().Name} is not mapped. Please check the mapping in the MatchingEngineSerializer class");
                }
            }
            catch (KeyNotFoundException exception)
            {
                if (_ignoreErrors)
                {
                    _logger?.Add($"Response: {data.ToJson()}");
                    _logger?.Add(exception.ToString());
                }
                else
                {
                    throw;
                }
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

        public Task Disconnect()
        {
            _tasksManager.SetExceptionsToAll(new Exception("Socket disconnected"));
            _newTasksManager.SetExceptionsToAll(new Exception("Socket disconnected"));
            _marketOrdersTasksManager.SetExceptionsToAll(new Exception("Socket disconnected"));
            _multiOrdersTasksManager.SetExceptionsToAll(new Exception("Socket disconnected"));
            return Task.CompletedTask;
        }
    }
}
