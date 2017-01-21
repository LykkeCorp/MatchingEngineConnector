using System.Threading.Tasks;
using Lykke.MatchingEngine.Connector.Abstractions.Models;

namespace Lykke.MatchingEngine.Connector.Abstractions.Services
{
    /// <summary>
    /// Connector for Matching Engine
    /// </summary>
    public interface IMatchingEngineConnector
    {
        Task<string> HandleMarketOrderAsync(string clientId, string assetPairId,
            OrderAction orderAction, double volume, bool straight);

        Task HandleLimitOrderAsync(string clientId, string assetPairId,
            OrderAction orderAction, double volume, double price);

        /// <summary>
        /// Transfer some amount of an asset, from one client to another
        /// </summary>
        /// <param name="fromClientId">Source client id</param>
        /// <param name="toClientId">Target client id</param>
        /// <param name="assetId">Asset id</param>
        /// <param name="amount">Amount to be transfered</param>
        /// <param name="businessId">internal id of transaction, to prevent double sending</param>
        /// <returns></returns>
        Task TransferAsync(string fromClientId, string toClientId,
            string assetId, double amount, string businessId);

        Task<CashInOutResponse> CashInOutBalanceAsync(string clientId, string assetId,
            double balanceDelta, bool sendToBlockchain, string correlationId);

        Task UpdateBalanceAsync(string clientId, string assetId, double value);

        Task CancelLimitOrderAsync(int orderId);

        /// <summary>
        /// Update Wallet Credentials cache in ME
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>True if update was successful</returns>
        Task<bool> UpdateWalletCredsForClient(string clientId);
    }
}