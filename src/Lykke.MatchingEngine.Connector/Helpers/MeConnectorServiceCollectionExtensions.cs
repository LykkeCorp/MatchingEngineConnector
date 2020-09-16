using System;
using System.Net;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Services;
using JetBrains.Annotations;
using Lykke.Common.Log;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Matching engine connector extensions for <see cref="IServiceCollection"/>
    /// </summary>
    [PublicAPI]
    public static class MeConnectorServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="IMatchingEngineClient"/> in the <paramref name="services"/>
        /// </summary>
        /// <remarks>
        /// <see cref="ILogFactory"/> should be registered in the container.
        /// </remarks>
        /// <param name="services">Autofac container builder</param>
        /// <param name="ipEndPoint">ME IP endpoint</param>
        [Obsolete("Use RegisterMeClient overload with ignoreErrors flag")]
        public static void RegisterMeClient(
            this IServiceCollection services,
            IPEndPoint ipEndPoint)
        {
            services.AddSingleton(s =>
            {
                var client = new TcpMatchingEngineClient(ipEndPoint, s.GetRequiredService<ILogFactory>());

                client.Start();

                return client;
            });
            services.AddSingleton<IMatchingEngineClient>(s => s.GetRequiredService<TcpMatchingEngineClient>());
        }

        /// <summary>
        /// Registers <see cref="IMatchingEngineClient"/> in the <paramref name="services"/>
        /// </summary>
        /// <remarks><see cref="ILogFactory"/> should be registered in the container.</remarks>
        /// <param name="services">Autofac container builder</param>
        /// <param name="ipEndPoint">ME IP endpoint</param>
        /// <param name="ignoreErrors">Flag indicating, if unknown response error should not rise exception</param>
        public static void RegisterMeClient(
            this IServiceCollection services,
            IPEndPoint ipEndPoint,
            bool ignoreErrors)
        {
            services.AddSingleton(s =>
            {
                var client = new TcpMatchingEngineClient(
                    ipEndPoint,
                    s.GetRequiredService<ILogFactory>(),
                    ignoreErrors);

                client.Start();

                return client;
            });
            services.AddSingleton<IMatchingEngineClient>(s => s.GetRequiredService<TcpMatchingEngineClient>());
        }
    }
}
