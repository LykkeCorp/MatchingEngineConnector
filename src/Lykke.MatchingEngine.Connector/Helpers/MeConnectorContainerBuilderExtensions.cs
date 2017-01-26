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
        public static void BindMeConnector(this ContainerBuilder ioc,
            IPEndPoint ipEndPoint, ISocketLog socketLog = null)
        {
            if (socketLog == null)
                socketLog = new SocketLogDynamic(i => { },
                    str => Console.WriteLine(DateTime.UtcNow.ToIsoDateTime() + ": " + str)
                );

            var tcpClient = new TcpClientMatchingEngineConnector(ipEndPoint, socketLog);
            ioc.RegisterInstance(tcpClient)
                .As<IMatchingEngineConnector>()
                .As<TcpClientMatchingEngineConnector>();

            tcpClient.Start();
        }
    }
}