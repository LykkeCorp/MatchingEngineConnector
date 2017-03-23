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
        [Obsolete("This interface is obsolete. Use AddMeClient instead.")]
        public static void AddMeConnector(this IServiceCollection services,
            IPEndPoint ipEndPoint, ISocketLog socketLog = null)
        {
            if (socketLog == null)
                socketLog = new SocketLogDynamic(i => { },
                    str => Console.WriteLine(DateTime.UtcNow.ToIsoDateTime() + ": " + str)
                );

            var tcpClient = new TcpClientMatchingEngineConnector(ipEndPoint, socketLog);
            services.AddSingleton<IMatchingEngineConnector>(tcpClient);
            services.AddSingleton<TcpClientMatchingEngineConnector>(tcpClient);

            tcpClient.Start();
        }

        public static void AddMeClient(this IServiceCollection services,
            IPEndPoint ipEndPoint, ISocketLog socketLog = null)
        {
            if (socketLog == null)
                socketLog = new SocketLogDynamic(i => { },
                    str => Console.WriteLine(DateTime.UtcNow.ToIsoDateTime() + ": " + str)
                );

            var tcpMeClient = new TcpMatchingEngineClient(ipEndPoint, socketLog);
            services.AddSingleton<IMatchingEngineClient>(tcpMeClient);
            services.AddSingleton<TcpMatchingEngineClient>(tcpMeClient);

            tcpMeClient.Start();
        }
    }
}