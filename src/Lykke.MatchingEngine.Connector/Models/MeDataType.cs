namespace Lykke.MatchingEngine.Connector.Models
{
    public enum MeDataType
    {
        TheResponse,
        Ping,
        UpdateBalance,
        LimitOrder,
        MarketOrderObsolete,
        LimitOrderCancel,
        BalanceUpdate,
        MultiLimitOrder,
        Transfer,
        CashInOut,
        Swap,
        WalletCredentialsReload = 20,
        NewLimitOrder = 50,
        NewMarketOrder = 52,
        NewLimitOrderCancel = 55,
        TheNewResponse = 99,
        MarketOrder = 53,
        MarketOrderResponse = 100
    }
}