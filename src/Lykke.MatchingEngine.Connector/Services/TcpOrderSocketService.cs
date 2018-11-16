using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Models.Me;

namespace Lykke.MatchingEngine.Connector.Services
{
    internal class TcpOrderSocketService : ITcpClientService, ISocketNotifier
    {
        private readonly TasksManager<TheResponseModel> _tasksManager;
        private readonly TasksManager<TheNewResponseModel> _newTasksManager;
        private readonly TasksManager<MarketOrderResponseModel> _marketOrdersTasksManager;
        private readonly TasksManager<MeMultiLimitOrderResponseModel> _multiOrdersTasksManager;
        [Obsolete]
        [CanBeNull]
        private readonly ISocketLog _legacyLog;
        private readonly bool _ignoreErrors;
        [CanBeNull]
        private readonly ILog _log;

        [Obsolete]
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
            _legacyLog = logger;
            _ignoreErrors = ignoreErrors;
        }

        public TcpOrderSocketService(TasksManager<TheResponseModel> tasksManager,
            TasksManager<TheNewResponseModel> newTasksManager,
            TasksManager<MarketOrderResponseModel> marketOrdersTasksManager,
            TasksManager<MeMultiLimitOrderResponseModel> multiOrdersTasksManager,
            ILogFactory logFactory)
        {
            _tasksManager = tasksManager;
            _newTasksManager = newTasksManager;
            _marketOrdersTasksManager = marketOrdersTasksManager;
            _multiOrdersTasksManager = multiOrdersTasksManager;
            _log = logFactory.CreateLog(this);
        }

        public void HandleDataFromSocket(object data)
        {
            try
            {
                switch (data)
                {
                    case TheResponseModel theResponse:
                        _log?.Info("Response", new {processId = theResponse.ProcessId, responseType = theResponse.GetType()});
                        _legacyLog?.Add($"Response ProcessId: {theResponse.ProcessId}");
                        _tasksManager.SetResult(theResponse.ProcessId, theResponse);
                        break;

                    case TheNewResponseModel theNewResponse:
                        _log?.Info("Response", new {processId = theNewResponse.Id, responseType = theNewResponse.GetType()});
                        _legacyLog?.Add($"Response Id: {theNewResponse.Id}");
                        _newTasksManager.SetResult(theNewResponse.Id, theNewResponse);
                        break;

                    case MarketOrderResponseModel theMarketOrderResponse:
                        _log?.Info("Response", new {processId = theMarketOrderResponse.Id, responseType = theMarketOrderResponse.GetType()});
                        _legacyLog?.Add($"Response Id: {theMarketOrderResponse.Id}");
                        _marketOrdersTasksManager.SetResult(theMarketOrderResponse.Id, theMarketOrderResponse);
                        break;

                    case MeMultiLimitOrderResponseModel multiLimitOrderResponse:
                        _log?.Info("Response", new {processId = multiLimitOrderResponse.Id, responseType = multiLimitOrderResponse.GetType()});
                        _legacyLog?.Add($"Response Id: {multiLimitOrderResponse.Id}");
                        _multiOrdersTasksManager.SetResult(multiLimitOrderResponse.Id, multiLimitOrderResponse);
                        break;

                    // No handlers for the following messages
                    case MePingModel m0:
                    case MeCashInOutModel m1:
                    case MeLimitOrderModel m2:
                    case MeMarketOrderObsoleteModel m3:
                    case MeLimitOrderCancelModel m4:
                    case MeUpdateBalanceModel m5:
                    case MeNewTransferModel m6:
                    case MeNewCashInOutModel m7:
                    case MeNewSwapModel m8:
                    case MeUpdateWalletCredsModel m9:
                    case MeNewLimitOrderModel m10:
                    case MeNewLimitOrderCancelModel m11:
                    case MeNewMarketOrderModel m12:
                    case MeMarketOrderModel m13:
                    case MeNewUpdateBalanceModel m14:
                    case MeMultiLimitOrderModel m15:
                    case MeMultiLimitOrderCancelModel m16:
                    case MeNewUpdateReservedBalanceModel m17:
                        break;
                    default:
                        throw new ArgumentException(nameof(data), $"{data.GetType().Name} is not mapped. Please check the mapping in the MatchingEngineSerializer class");
                }
            }
            catch (KeyNotFoundException exception)
            {
                _log?.Error(exception, context: data);

                if (_ignoreErrors)
                {
                    _legacyLog?.Add($"Response: {data.ToJson()}");
                    _legacyLog?.Add(exception.ToString());
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
