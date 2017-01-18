using System;
using System.Net;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Services;
using Common;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class MeConnectorServiceCollectionExtensions
    {
        public static void AddMeConnector(this IServiceCollection services,
            IPEndPoint ipEndPoint, ISocketLog socketLog = null)
        {
            if (socketLog == null)
                socketLog = new SocketLogDynamic(i => { },
                    str => Console.WriteLine(DateTime.UtcNow.ToIsoDateTime() + ": " + str)
                );

            var tcpClient = new TcpClientMatchingEngineConnector(ipEndPoint, socketLog);
            services.AddSingleton<IMatchingEngineConnector>(tcpClient);
            tcpClient.Start();
        }
    }
}