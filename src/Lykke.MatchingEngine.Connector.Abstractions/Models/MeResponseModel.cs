namespace Lykke.MatchingEngine.Connector.Abstractions.Models
{
    public enum MeStatusCodes
    {
        Ok = 0,

        //validation errors
        LowBalance = 401,
        AlreadyProcessed = 402,

        //ME errors
        Runtime = 501
    }

    public class MeResponseModel
    {
        public MeStatusCodes Status { get; set; }
        public string Message { get; set; }
    }
}
