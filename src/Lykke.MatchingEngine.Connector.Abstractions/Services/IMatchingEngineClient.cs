using System;
using System.Threading.Tasks;
using Lykke.MatchingEngine.Connector.Abstractions.Models;

namespace Lykke.MatchingEngine.Connector.Abstractions.Services
{
    /// <summary>
    /// Client for Matching Engine
    /// </summary>
    public interface IMatchingEngineClient
    {
        /// <summary>
        /// Property specifying if ME Client is Connected to ME
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Manually set asset balance for a client
        /// </summary>
        /// /// <param name="id">internal id of transaction, to prevent double sending and further processing</param>
        /// <param name="clientId">Id of the client</param>
        /// <param name="assetId">Id of the asset</param>
        /// <param name="value">New balance value</param>
        /// <returns></returns>
        Task UpdateBalanceAsync(string id, string clientId, string assetId, double value);

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
        /// <param name="amount1">First amount</param>
        /// <param name="clientId2">Second client id</param>
        /// <param name="assetId2">Second asset id</param>
        /// <param name="amount2">Second amount</param>
        /// <returns>Status code and message</returns>
        Task<MeResponseModel> SwapAsync(string id,
            string clientId1, string assetId1, double amount1,
            string clientId2, string assetId2, double amount2);

        /// <summary>
        /// Place a limit order on the matching engine
        /// </summary>
        /// <param name="id">internal id of transaction, to prevent double sending and further processing</param>
        /// <param name="clientId">Id of the client</param>
        /// <param name="assetPairId">Id of the Asset Pair</param>
        /// <param name="orderAction">Type of Order action (buy/sell)</param>
        /// <param name="volume">Amount of base asset (to buy or sell)</param>
        /// <param name="price">Price for base asset in quoting asset</param>
        /// <param name="cancelPreviousOrders">Cancels all previous limit orders of the client</param>
        /// <returns></returns>
        Task<MeResponseModel> PlaceLimitOrderAsync(LimitOrderModel model);

        /// <summary>
        /// Cancel previously placed Limit order
        /// </summary>
        /// <param name="limitOrderId">id of the limit order to be canceled</param>
        /// <returns></returns>
        Task<MeResponseModel> CancelLimitOrderAsync(string limitOrderId);

        /// <summary>
        /// Handles market order, Matches with limit order if available
        /// </summary>
        /// <param name="clientId">id of the client</param>
        /// <param name="assetPairId">id of the asset pair</param>
        /// <param name="orderAction">Type of Order action (buy/sell)</param>
        /// <param name="volume">Amount of base asset (to buy or sell)</param>
        /// <param name="straight"></param>
        /// <param name="reservedLimitVolume"></param>
        /// <returns></returns>
        Task<string> HandleMarketOrderAsync(string clientId, string assetPairId,
            OrderAction orderAction, double volume, bool straight, double? reservedLimitVolume = null);

        /// <summary>
        /// Handles market order, Matches with limit order if available
        /// </summary>
        Task<MarketOrderResponse> HandleMarketOrderAsync(MarketOrderModel model);
    }
}