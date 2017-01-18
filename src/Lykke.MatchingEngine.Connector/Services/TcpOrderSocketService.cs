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

        public TcpOrderSocketService(TasksManager<long, TheResponseModel> tasksManager)
        {
            _tasksManager = tasksManager;
        }

        public Task HandleDataFromSocket(object data)
        {
            var theResponse = data as TheResponseModel;

            if (theResponse != null)
            {
                _tasksManager.Compliete(theResponse.ProcessId, theResponse);
                Console.WriteLine("Response ProcessId: " + theResponse.ProcessId);
            }


            return Task.FromResult(0);
        }

        public Func<object, Task> SendDataToSocket { get; set; }
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
            return Task.FromResult(0);
        }
    }
}
