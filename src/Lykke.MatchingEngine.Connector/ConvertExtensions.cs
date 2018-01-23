using Lykke.MatchingEngine.Connector.Models;

namespace Lykke.MatchingEngine.Connector
{
    public static class ConvertExtensions
    {
        public static Fee ToApiModel(this Domain.Fee src)
        {
            return new Fee
            {
                Size = src.Size,
                SizeType = (int) src.SizeType,
                SourceClientId = src.SourceClientId,
                TargetClientId = src.TargetClientId,
                Type = (int) src.Type
            };
        }
    }
}