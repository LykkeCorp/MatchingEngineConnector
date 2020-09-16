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
        /// Registers <see cref="IMatchingEngineClient"/> in <paramref name="ioc"/>
        /// </summary>
        /// <remarks>
        /// <see cref="ILogFactory"/> should be registered in the container.
        /// </remarks>
        /// <param name="ioc">Autofac container builder</param>
        /// <param name="ipEndPoint">ME IP endpoint</param>
        [Obsolete("Use RegisterMeClient overload with ignoreErrors flag")]
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
                .As<TcpMatchingEngineClient>()
                .SingleInstance();
        }

        /// <summary>
        /// Registers <see cref="IMatchingEngineClient"/> in <paramref name="ioc"/>
        /// </summary>
        /// <remarks><see cref="ILogFactory"/> should be registered in the container.</remarks>
        /// <param name="ioc">Autofac container builder</param>
        /// <param name="ipEndPoint">ME IP endpoint</param>
        /// <param name="ignoreErrors">Flag indicating, if unknown response error should not rise exception</param>
        public static void RegisterMeClient(
            this ContainerBuilder ioc,
            IPEndPoint ipEndPoint,
            bool ignoreErrors = false)
        {
            ioc.Register(s =>
                {
                    var tcpMeClient = new TcpMatchingEngineClient(
                        ipEndPoint,
                        s.Resolve<ILogFactory>(),
                        ignoreErrors);

                    tcpMeClient.Start();

                    return tcpMeClient;
                })
                .As<IMatchingEngineClient>()
                .As<TcpMatchingEngineClient>()
                .SingleInstance();
        }

        /// <summary>
        /// Registers <see cref="IMatchingEngineClient"/> in <paramref name="ioc"/>
        /// </summary>
        /// <remarks><see cref="ILogFactory"/> should be registered in the container.</remarks>
        /// <param name="ioc">Autofac container builder</param>
        /// <param name="settings">ME connection settings</param>
        public static void RegisterMeClient(
            this ContainerBuilder ioc,
            MeClientSettings settings)
        {
            ioc.Register(s =>
                {
                    var tcpMeClient = new TcpMatchingEngineClient(
                        settings,
                        s.Resolve<ILogFactory>());

                    tcpMeClient.Start();

                    return tcpMeClient;
                })
                .As<IMatchingEngineClient>()
                .As<TcpMatchingEngineClient>()
                .SingleInstance();
        }
    }
}
