namespace Lykke.MatchingEngine.Connector.Models.RabbitMq
{
    /// <summary>
    /// Fee for limit order trades.
    /// </summary>
    public class LimitFee
    {
        /// <summary>Fee instruction for limit tarde.</summary>
        public LimitFeeInstruction Instruction { get; set; }

        /// <summary>Fee transfer for limit tarde.</summary>
        public FeeTransfer Transfer { get; set; }
    }
}
