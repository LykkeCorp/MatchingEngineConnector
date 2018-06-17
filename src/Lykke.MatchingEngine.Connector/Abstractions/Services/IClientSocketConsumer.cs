namespace Lykke.MatchingEngine.Connector.Abstractions.Services
{
    public interface IClientSocketConsumer<out TService>
        where TService : ITcpService
    {
        TService GetConnection();
    }
}