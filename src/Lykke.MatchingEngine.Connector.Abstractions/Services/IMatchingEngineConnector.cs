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
        [Obsolete]
        Task<string> HandleMarketOrderObsoleteAsync(string clientId, string assetPairId,
            OrderAction orderAction, double volume, bool straight, double? reservedLimitVolume = null);

        [Obsolete]
        Task<string> HandleMarketOrderObsoleteAsync(string id, string clientId, string assetPairId,
            OrderAction orderAction, double volume, bool straight, double? reservedLimitVolume = null);

        Task<MarketOrderResponse> HandleMarketOrderAsync(MarketOrderModel model);

        Task<MeResponseModel> HandleLimitOrderAsync(LimitOrderModel model);

        Task<MeResponseModel> CancelLimitOrderAsync(string limitOrderId);

        [Obsolete("This method is depricated and will be removed in future releases. Please use CashInOutAsync instead.")]
        Task<CashInOutResponse> CashInOutBalanceAsync(string clientId, string assetId,
            double balanceDelta, bool sendToBlockchain, string correlationId);

        Task UpdateBalanceAsync(string id, string clientId, string assetId, double value);

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
        /// <returns>Status code and message</returns>
        Task<MeResponseModel> CashInOutAsync(string id, string clientId, string assetId, double amount);

        /// <summary>
        /// Cash In or Out some amount of an asset
        /// </summary>
        /// <param name="id">internal id of transaction, to prevent double sending and further processing</param>
        /// <param name="clientId">Client id</param>
        /// <param name="assetId">Asset id</param>
        /// <param name="amount">Amount to be cashed in or out</param>
        /// <param name="feeClientId">Fee client id</param>
        /// <param name="feeSize">Size of fee (0.01 = 1%, 1.0 = 100%)</param>
        /// <param name="feeSizeType">Type of fee size (PERCENTAGE or ABSOLUTE)</param>
        /// <returns>Status code and message</returns>
        Task<MeResponseModel> CashInOutAsync(string id, string clientId, string assetId, double amount, string feeClientId, double feeSize, FeeSizeType feeSizeType);

        /// <summary>
        /// Transfer some amount of an asset, from one client to another
        /// </summary>
        /// <param name="id">internal id of transaction, to prevent double sending and further processing</param>
        /// <param name="fromClientId">Source client id</param>
        /// <param name="toClientId">Target client id</param>
        /// <param name="assetId">Asset id</param>
        /// <param name="amount">Amount to be transfered</param>
        /// <param name="feeClientId">Fee client id</param>
        /// <param name="feeSizePercentage">Fee amount (1.0 is 100%, 0.01 is 1%)</param>
        /// <returns>Status code and message</returns>
        Task<MeResponseModel> TransferAsync(string id, string fromClientId,
            string toClientId, string assetId, double amount, string feeClientId, double feeSizePercentage, double overdraft);

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
        /// <returns>Status code and message</returns>
        Task<MeResponseModel> SwapAsync(string id,
            string clientId1, string assetId1, double amount1,
            string clientId2, string assetId2, double amount2);
    }
}