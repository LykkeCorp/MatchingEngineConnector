namespace Lykke.MatchingEngine.Connector.Models.Common
{
    /// <summary>
    /// Adds validation check to a model.
    /// </summary>
    public interface IValidatable
    {
        /// <summary>
        /// Checks model validity.
        /// </summary>
        /// <returns>Validation bool result</returns>
        bool IsValid();
    }
}
