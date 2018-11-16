using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lykke.MatchingEngine.Connector.Models.Common;
using Lykke.MatchingEngine.Connector.Models.Api;

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
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns></returns>
        Task UpdateBalanceAsync(
            string id,
            string clientId,
            string assetId,
            double value,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Manually set reserved balance for a client
        /// </summary>
        /// <param name="id">Internal Id of transaction for identification and deduplication</param>
        /// <param name="clientId">Id of the client</param>
        /// <param name="assetId">Id of the asset</param>
        /// <param name="amount">New reserved balance value</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns></returns>
        Task<MeResponseModel> UpdateReservedBalanceAsync(
            string id,
            string clientId,
            string assetId,
            double amount,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cash In or Out some amount of an asset
        /// </summary>
        /// <param name="id">internal id of transaction, to prevent double sending and further processing</param>
        /// <param name="clientId">Client id</param>
        /// <param name="assetId">Asset id</param>
        /// <param name="amount">Amount to be cashed in or out</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>Status code and message</returns>
        Task<MeResponseModel> CashInOutAsync(
            string id,
            string clientId,
            string assetId,
            double amount,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cash In or Out some amount of an asset
        /// </summary>
        /// <param name="id">internal id of transaction, to prevent double sending and further processing</param>
        /// <param name="clientId">Client id</param>
        /// <param name="assetId">Asset id</param>
        /// <param name="accuracy">Asset accuracy</param>
        /// <param name="amount">Amount to be cashed in or out</param>
        /// <param name="feeClientId">Fee client id</param>
        /// <param name="feeSize">Size of fee (0.01 = 1%, 1.0 = 100%)</param>
        /// <param name="feeSizeType">Type of fee size (PERCENTAGE or ABSOLUTE)</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>Status code and message</returns>
        Task<MeResponseModel> CashInOutAsync(
            string id,
            string clientId,
            string assetId,
            int accuracy,
            double amount,
            string feeClientId,
            double feeSize,
            FeeSizeType feeSizeType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Transfer some amount of an asset, from one client to another
        /// </summary>
        /// <param name="id">internal id of transaction, to prevent double sending and further processing</param>
        /// <param name="fromClientId">Source client id</param>
        /// <param name="toClientId">Target client id</param>
        /// <param name="assetId">Asset id</param>
        /// <param name="accuracy">Asset accuracy</param>
        /// <param name="amount">Amount to be transfered</param>
        /// <param name="feeModel">Fee info</param>
        /// <param name="overdraft"></param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>Status code and message</returns>
        Task<MeResponseModel> TransferAsync(
            string id,
            string fromClientId,
            string toClientId,
            string assetId,
            int accuracy,
            double amount,
            FeeModel feeModel,
            double overdraft,
            CancellationToken cancellationToken = default);

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
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>Status code and message</returns>
        Task<MeResponseModel> SwapAsync(
            string id,
            string clientId1,
            string assetId1,
            double amount1,
            string clientId2,
            string assetId2,
            double amount2,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Place a limit order on the matching engine
        /// </summary>
        /// <param name="model">A limit order</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns></returns>
        Task<MeResponseModel> PlaceLimitOrderAsync(LimitOrderModel model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Place a stop limit order on the matching engine
        /// </summary>
        /// <param name="model">A stop limit order</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns></returns>
        Task<MeResponseModel> PlaceStopLimitOrderAsync(StopLimitOrderModel model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancel previously placed Limit order
        /// </summary>
        /// <param name="limitOrderId">id of the limit order to be canceled</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns></returns>
        Task<MeResponseModel> CancelLimitOrderAsync(string limitOrderId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancel previously placed Limit orders
        /// </summary>
        /// <param name="limitOrderId">id list of the limit orders to be canceled</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns></returns>
        Task<MeResponseModel> CancelLimitOrdersAsync(IEnumerable<string> limitOrderId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles market order, Matches with limit order if available
        /// </summary>
        /// <param name="clientId">id of the client</param>
        /// <param name="assetPairId">id of the asset pair</param>
        /// <param name="orderAction">Type of Order action (buy/sell)</param>
        /// <param name="volume">Amount of base asset (to buy or sell)</param>
        /// <param name="straight"></param>
        /// <param name="reservedLimitVolume"></param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns></returns>
        Task<string> HandleMarketOrderAsync(
            string clientId,
            string assetPairId,
            OrderAction orderAction,
            double volume,
            bool straight,
            double? reservedLimitVolume = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles market order, Matches with limit order if available
        /// <param name="model">A market order</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// </summary>
        Task<MarketOrderResponse> HandleMarketOrderAsync(MarketOrderModel model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Place multiple limit orders
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns></returns>
        Task<MultiLimitOrderResponse> PlaceMultiLimitOrderAsync(MultiLimitOrderModel model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancel multiple limit orders
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns></returns>
        Task<MeResponseModel> CancelMultiLimitOrderAsync(MultiLimitOrderCancelModel model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels limit orders
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MeResponseModel> MassCancelLimitOrdersAsync(
            LimitOrderMassCancelModel model,
            CancellationToken cancellationToken = default);
    }
}
