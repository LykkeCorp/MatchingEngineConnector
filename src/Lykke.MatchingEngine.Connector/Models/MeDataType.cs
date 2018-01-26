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
        MultiLimitOrder = 51,
        Transfer,
        CashInOut,
        Swap,
        WalletCredentialsReload = 20,
        NewLimitOrder = 50,
        NewMarketOrder = 52,
        NewLimitOrderCancel = 55,
        TheNewResponse = 99,
        MarketOrder = 53,
        MarketOrderResponse = 100,
        UpdateBalanceNew = 11,
        MultiLimitOrderCancel = 57,
        MultiLimitOrderResponse = 98
    }
}