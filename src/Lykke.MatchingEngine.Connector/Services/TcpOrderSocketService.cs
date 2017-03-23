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

        public TcpOrderSocketService(TasksManager<long, TheResponseModel> tasksManager,
            TasksManager<string, TheNewResponseModel> newTasksManager)
        {
            _tasksManager = tasksManager;
            _newTasksManager = newTasksManager;
        }

        public async Task HandleDataFromSocket(object data)
        {
            await Task.Run(() =>
            {
                var theResponse = data as TheResponseModel;
                if (theResponse != null)
                {
                    _tasksManager.Compliete(theResponse.ProcessId, theResponse);
                    Console.WriteLine($"Response ProcessId: {theResponse.ProcessId}");
                    return;
                }

                var theNewResponse = data as TheNewResponseModel;
                if (theNewResponse != null)
                {
                    _newTasksManager.Compliete(theNewResponse.Id, theNewResponse);
                    Console.WriteLine($"Response Id: {theNewResponse.Id}");
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
            return Task.FromResult(0);
        }
    }
}
