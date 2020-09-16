using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Logs;
using Lykke.MatchingEngine.Connector.Models.Api;
using Lykke.MatchingEngine.Connector.Models.Common;
using Lykke.MatchingEngine.Connector.Services;

namespace TestClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new TcpMatchingEngineClient(
                new MeClientSettings
                {
                    Endpoint = new IPEndPoint(
                        IPAddress.Parse(Dns.GetHostAddresses("me.me.svc.cluster.local")[0].ToString()), 8888)
                }, EmptyLogFactory.Instance);

            Thread.Sleep(100);
            client.Start();
            Thread.Sleep(100);

            var res = await client.PlaceLimitOrderAsync(
                new LimitOrderModel
                {
                    Id = Guid.NewGuid().ToString(),
                    AssetPairId = "BTCUSD",
                    ClientId = "954dd309-1f64-481f-9059-99d541bd4349",
                    OrderAction = OrderAction.Buy,
                    Price = 12000,
                    Volume = 0.001

                });

            Console.WriteLine(res.Status);
        }
    }
}
