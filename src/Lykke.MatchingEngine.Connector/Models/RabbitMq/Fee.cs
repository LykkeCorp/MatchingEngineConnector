namespace Lykke.MatchingEngine.Connector.Models.RabbitMq
{
    /// <summary>
    /// Fee.
    /// </summary>
    /// <see cref="https://github.com/LykkeCity/MatchingEngine/blob/master/src/main/kotlin/com/lykke/matching/engine/daos/fee/Fee.kt"/>
    public class Fee
    {
        /// <summary>Fee instruction.</summary>
        public FeeInstruction Instruction { get; set; }

        /// <summary>Fee transfer.</summary>
        public FeeTransfer Transfer { get; set; }
    }
}
