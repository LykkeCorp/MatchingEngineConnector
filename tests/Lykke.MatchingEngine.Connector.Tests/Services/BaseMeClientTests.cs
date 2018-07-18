using System;
using System.Net;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Services;

namespace Lykke.MatchingEngine.Connector.Tests.Services
{
    public class BaseMeClientTests
    {
        public const string Url = "";
        public const string ClientId = "";

        public static string GenerateUid()
        {
            return Guid.NewGuid().ToString();
        }

        public static IMatchingEngineClient CreateConnection(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url), "Url must be specified");
            }

            var client =
                new TcpMatchingEngineClient(new IPEndPoint(IPAddress.Parse(Dns.GetHostAddresses(url)[0].ToString()),
                    8888));
            client.Start();
            return client;
        }
    }
}