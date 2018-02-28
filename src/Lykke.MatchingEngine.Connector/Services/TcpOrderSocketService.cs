using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Models;

namespace Lykke.MatchingEngine.Connector.Services
{
    internal class TcpOrderSocketService : ITcpClientService, ISocketNotifyer
    {
        private readonly TasksManager<long, TheResponseModel> _tasksManager;
        private readonly TasksManager<string, TheNewResponseModel> _newTasksManager;
        private readonly TasksManager<string, MarketOrderResponseModel> _marketOrdersTasksManager;
        private readonly TasksManager<string, MeMultiLimitOrderResponseModel> _multiOrdersTasksManager;
        private readonly ISocketLog _logger;
        private readonly bool _ignoreErrors;

        public TcpOrderSocketService(TasksManager<long, TheResponseModel> tasksManager,
            TasksManager<string, TheNewResponseModel> newTasksManager,
            TasksManager<string, MarketOrderResponseModel> marketOrdersTasksManager,
            TasksManager<string, MeMultiLimitOrderResponseModel> multiOrdersTasksManager,
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

        public async Task HandleDataFromSocket(object data)
        {
            await Task.Run(() =>
            {
                try
                {
                    var theResponse = data as TheResponseModel;
                    if (theResponse != null)
                    {
                        _logger?.Add($"Response ProcessId: {theResponse.ProcessId}");
                        _tasksManager.Compliete(theResponse.ProcessId, theResponse);
                        return;
                    }

                    var theNewResponse = data as TheNewResponseModel;
                    if (theNewResponse != null)
                    {
                        _logger?.Add($"Response Id: {theNewResponse.Id}");
                        _newTasksManager.Compliete(theNewResponse.Id, theNewResponse);
                    }

                    var theMarketOrderResponse = data as MarketOrderResponseModel;
                    if (theMarketOrderResponse != null)
                    {
                        _logger?.Add($"Response Id: {theMarketOrderResponse.Id}");
                        _marketOrdersTasksManager.Compliete(theMarketOrderResponse.Id, theMarketOrderResponse);
                    }

                    var multiLimitOrderResponse = data as MeMultiLimitOrderResponseModel;
                    if (multiLimitOrderResponse != null)
                    {
                        _logger?.Add($"Response Id: {multiLimitOrderResponse.Id}");
                        _multiOrdersTasksManager.Compliete(multiLimitOrderResponse.Id, multiLimitOrderResponse);
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
            });
        }

        public Func<object, Task<bool>> SendDataToSocket { get; set; }
        public string ContextName => "TcpSocket";
        public object GetPingData()
        {
            return MePingModel.Instance;
        }

        public Task Connect()
        {
            return Task.FromResult(0);
        }

        public Task Disconnect()
        {
            _tasksManager.SetExceptionsToAll(new Exception("Socket disconnected"));
            _newTasksManager.SetExceptionsToAll(new Exception("Socket disconnected"));
            _marketOrdersTasksManager.SetExceptionsToAll(new Exception("Socket disconnected"));
            _multiOrdersTasksManager.SetExceptionsToAll(new Exception("Socket disconnected"));
            return Task.FromResult(0);
        }
    }
}
