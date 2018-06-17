namespace Lykke.MatchingEngine.Connector.Abstractions.Services
{
    public interface ITcpClientService : ITcpService
    {
        object GetPingData();
    }
}