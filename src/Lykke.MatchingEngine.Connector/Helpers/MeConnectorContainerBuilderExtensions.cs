using System;
using System.Net;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Services;
using Common;
using JetBrains.Annotations;
using Lykke.Common.Log;

// ReSharper disable once CheckNamespace
namespace Autofac
{
    /// <summary>
    /// Matching engine connector extensions for <see cref="ContainerBuilder"/>
    /// </summary>
    [PublicAPI]
    public static class MeConnectorContainerBuilderExtensions
    {
        /// <summary>
        /// Registers <see cref="IMatchingEngineClient"/> in the <paramref name="ioc"/>
        /// </summary>
        /// <param name="ioc">Autofac container builder</param>
        /// <param name="ipEndPoint">ME IP endpoint</param>
        /// <param name="socketLog">Log</param>
        /// <param name="ignoreErrors">Flag indicating, if response errors should be not written to the log</param>
        [Obsolete("Use RegisgterMeClient to use with new Lykke loging system")]
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

        /// <summary>
        /// Registers <see cref="IMatchingEngineClient"/> in <paramref name="ioc"/>
        /// </summary>
        /// <remarks>
        /// <see cref="ILogFactory"/> should be registered in the container.
        /// </remarks>
        /// <param name="ioc">Autofac container builder</param>
        /// <param name="ipEndPoint">ME IP endpoint</param>
        public static void RegisgterMeClient(
            this ContainerBuilder ioc,
            IPEndPoint ipEndPoint)
        {
            ioc.Register(s =>
                {
                    var tcpMeClient = new TcpMatchingEngineClient(ipEndPoint, s.Resolve<ILogFactory>());

                    tcpMeClient.Start();

                    return tcpMeClient;
                })
                .As<IMatchingEngineClient>()
                .As<TcpMatchingEngineClient>();
        }

    }
}