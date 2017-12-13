using System;
using System.Threading.Tasks;
using Common;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Models;

namespace Lykke.MatchingEngine.Connector.Services
{
    public class TcpOrderSocketService : ITcpClientService, ISocketNotifyer
    {
        private readonly TasksManager<long, TheResponseModel> _tasksManager;
        private readonly TasksManager<string, TheNewResponseModel> _newTasksManager;
        private readonly TasksManager<string, MarketOrderResponseModel> _marketOrdersTasksManager;
        private readonly bool _ignoreErrors;

        public TcpOrderSocketService(TasksManager<long, TheResponseModel> tasksManager,
            TasksManager<string, TheNewResponseModel> newTasksManager,
            TasksManager<string, MarketOrderResponseModel> marketOrdersTasksManager,
            bool ignoreErrors = false)
        {
            _tasksManager = tasksManager;
            _newTasksManager = newTasksManager;
            _marketOrdersTasksManager = marketOrdersTasksManager;
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
                        Console.WriteLine($"Response ProcessId: {theResponse.ProcessId}. Data: {theResponse.ToJson()}");
                        _tasksManager.Compliete(theResponse.ProcessId, theResponse);
                        return;
                    }

                    var theNewResponse = data as TheNewResponseModel;
                    if (theNewResponse != null)
                    {
                        Console.WriteLine($"Response Id: {theNewResponse.Id}. Data: {theNewResponse.ToJson()}");
                        _newTasksManager.Compliete(theNewResponse.Id, theNewResponse);
                    }

                    var theMarketOrderResponse = data as MarketOrderResponseModel;
                    if (theMarketOrderResponse != null)
                    {
                        Console.WriteLine($"Response Id: {theMarketOrderResponse.Id}. Data: {theMarketOrderResponse.ToJson()}");
                        _marketOrdersTasksManager.Compliete(theMarketOrderResponse.Id, theMarketOrderResponse);
                    }
                }
                catch (System.Collections.Generic.KeyNotFoundException exception)
                {
                    if (_ignoreErrors)
                    {
                        Console.WriteLine(exception);
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
            return Task.FromResult(0);
        }
    }
}
