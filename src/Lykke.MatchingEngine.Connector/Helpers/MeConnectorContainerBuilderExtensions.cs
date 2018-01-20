using System;
using System.Net;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Services;
using Common;

// ReSharper disable once CheckNamespace
namespace Autofac
{
    public static class MeConnectorContainerBuilderExtensions
    {
        public static void BindMeClient(this ContainerBuilder ioc,
            IPEndPoint ipEndPoint, ISocketLog socketLog = null, bool ignoreErrors = false)
        {
            if (socketLog == null)
                socketLog = new SocketLogDynamic(i => { },
                    str => Console.WriteLine(DateTime.UtcNow.ToIsoDateTime() + ": " + str)
                );

            var tcpMeClient = new TcpMatchingEngineClient(ipEndPoint, socketLog, ignoreErrors);
            ioc.RegisterInstance(tcpMeClient)
                .As<IMatchingEngineClient>()
                .As<TcpMatchingEngineClient>();

            tcpMeClient.Start();
        }
    }
}