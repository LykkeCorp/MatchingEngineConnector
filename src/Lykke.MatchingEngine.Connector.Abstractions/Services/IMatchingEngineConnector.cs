using System;
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

        [Obsolete("This method is depricated and will be removed in furutre releases. Please use CashInOutAsync instead.")]
        Task<CashInOutResponse> CashInOutBalanceAsync(string clientId, string assetId,
            double balanceDelta, bool sendToBlockchain, string correlationId);

        Task UpdateBalanceAsync(string clientId, string assetId, double value);

        Task CancelLimitOrderAsync(int orderId);

        /// <summary>
        /// Update Wallet Credentials cache in ME
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>True if update was successful</returns>
        [Obsolete("This method will be removed in future releases.")]
        Task<bool> UpdateWalletCredsForClient(string clientId);

        /// <summary>
        /// Cash In or Out some amount of an asset
        /// </summary>
        /// <param name="id">internal id of transaction, to prevent double sending and further processing</param>
        /// <param name="clientId">Client id</param>
        /// <param name="assetId">Asset id</param>
        /// <param name="amount">Amount to be cashed in or out</param>
        /// <returns>true if sucessfully send to ME</returns>
        Task<bool> CashInOutAsync(string id, string clientId, string assetId, double amount);

        /// <summary>
        /// Transfer some amount of an asset, from one client to another
        /// </summary>
        /// <param name="id">internal id of transaction, to prevent double sending and further processing</param>
        /// <param name="fromClientId">Source client id</param>
        /// <param name="toClientId">Target client id</param>
        /// <param name="assetId">Asset id</param>
        /// <param name="amount">Amount to be transfered</param>
        /// <returns>true if sucessfully send to ME</returns>
        Task<bool> TransferAsync(string id, string fromClientId,
            string toClientId, string assetId, double amount);

        /// <summary>
        /// Swap some assets between clients
        /// </summary>
        /// <param name="id">internal id of transaction, to prevent double sending and further processing</param>
        /// <param name="clientId1">First client id</param>
        /// <param name="assetId1">First asset id</param>
        /// <param name="amount1">First amount id</param>
        /// <param name="clientId2">Second client id</param>
        /// <param name="assetId2">Second asset id</param>
        /// <param name="amount2">Second amount id</param>
        /// <returns></returns>
        Task<bool> SwapAsync(string id,
            string clientId1, string assetId1, double amount1,
            string clientId2, string assetId2, double amount2);
    }
}