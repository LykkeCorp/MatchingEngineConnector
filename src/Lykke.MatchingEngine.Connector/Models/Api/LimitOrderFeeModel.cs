using System.Collections.Generic;
using Lykke.MatchingEngine.Connector.Models.Common;

namespace Lykke.MatchingEngine.Connector.Models.Api
{
    /// <summary>
    /// Limit order fee model.
    /// </summary>
    public class LimitOrderFeeModel
    {
        public FeeType Type { get; set; }

        public double MakerSize { get; set; }

        public double TakerSize { get; set; }

        public string SourceClientId { get; set; }

        public string TargetClientId { get; set; }

        public FeeSizeType MakerSizeType { get; set; }

        public FeeSizeType TakerSizeType { get; set; }

        public IReadOnlyList<string> AssetId { get; set; }

        public double MakerFeeModificator { get; set; }
    }
}
