namespace Lykke.MatchingEngine.Connector.Models
{
    public enum MeDataType
    {
        TheResponse, Ping, UpdateBalance, LimitOrder, MarketOrder, LimitOrderCancel, BalanceUpdate,
        WalletCredentialsReload = 20
    }
}