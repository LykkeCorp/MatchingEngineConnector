namespace Lykke.MatchingEngine.Connector.Models.Api
{
    /// <summary>
    /// ME response model.
    /// </summary>
    public class MeResponseModel
    {
        public MeStatusCodes Status { get; set; }

        public string Message { get; set; }

        public string TransactionId { get; set; }
    }
}
