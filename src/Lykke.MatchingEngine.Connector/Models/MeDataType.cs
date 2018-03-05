namespace Lykke.MatchingEngine.Connector.Models
{
    internal enum MeDataType : byte
    {
        TheResponse = 0,
        Ping = 1,
        UpdateBalance = 2,
        LimitOrder = 3,
        MarketOrderObsolete = 4,
        LimitOrderCancel = 5,
        BalanceUpdate = 6,
        MultiLimitOrder = 7,
        Transfer = 8,
        CashInOut = 9,
        Swap = 10,
        WalletCredentialsReload = 20,
        NewLimitOrder = 50,
        NewMultiLimitOrder = 51,
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