using Lykke.MatchingEngine.Connector.Models;

namespace Lykke.MatchingEngine.Connector
{
    internal static class ConvertExtensions
    {
        public static FeeContract ToApiModel(this Domain.Fee src)
        {
            return new FeeContract
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