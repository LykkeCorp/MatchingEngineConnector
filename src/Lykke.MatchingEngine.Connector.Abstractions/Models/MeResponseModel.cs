namespace Lykke.MatchingEngine.Connector.Abstractions.Models
{
    public enum MeStatusCodes
    {
        Ok = 0,

        //validation errors
        LowBalance = 401,
        AlreadyProcessed = 402,
        UnknownAsset = 410,
        NoLiquidity = 411,
        NotEnoughFunds = 412,
        Dust = 413,
        ReservedVolumeHigherThanBalance = 414,
        NotFound = 415,
        BalanceLowerThanReserved = 416,
        LeadToNegativeSpread = 417,
        //ME errors        
        Runtime = 500
    }

    public class MeResponseModel
    {
        public MeStatusCodes Status { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
    }
}
