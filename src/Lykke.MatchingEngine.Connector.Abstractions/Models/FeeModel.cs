using Lykke.MatchingEngine.Connector.Abstractions.Models;
using System;

namespace Lykke.MatchingEngine.Connector.Abstractions.Models
{
    //todo: Normally this logic has to be implemented inside FeeCalculator service

    public class FeeModel
    {
        public FeeType Type { get; set;  }

        public double Size { get; set; }

        public string SourceClientId { get; set; }

        public string TargetClientId { get; set; }

        public FeeSizeType SizeType { get; set; }

        public FeeChargingType ChargingType { get; set; }
    }
}