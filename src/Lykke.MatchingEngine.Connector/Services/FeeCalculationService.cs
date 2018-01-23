using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.MatchingEngine.Connector.Models;
using System;
using Common;

namespace Lykke.MatchingEngine.Connector.Services
{
    public class FeeCalculationService
    {
        public Domain.Fee GetCashInOutFee(double amount, int accuracy, string clientId, double feeSize = 0d,
            FeeSizeType feeSizeType = FeeSizeType.ABSOLUTE)
        {
            var feeAbsolute = 0d;

            if (feeSize > 0)
            {
                switch (feeSizeType)
                {
                    case FeeSizeType.ABSOLUTE:
                        feeAbsolute = feeSize;
                        break;
                    case FeeSizeType.PERCENTAGE:
                        feeAbsolute = Math.Round(amount * feeSize, 15);
                        break;
                    default: throw new Exception("Unknown fee size type");
                }

                feeAbsolute = feeAbsolute.TruncateDecimalPlaces(accuracy, true);
            }

            return new Domain.Fee
            {
                SourceClientId = null,
                TargetClientId = clientId,
                Size = feeAbsolute,
                Type = feeSize.GetFeeType(),
                SizeType = feeSizeType
            };
        }

        public Domain.Fee GetTransferFee(double amount, int accuracy, string feeClientId, double feeSizePercentage)
        {
            var feeAbsolute = Math.Round(amount * feeSizePercentage, 15).TruncateDecimalPlaces(accuracy, true);

            return new Domain.Fee
            {
                SourceClientId = null,
                TargetClientId = feeClientId,
                Size = feeAbsolute,
                Type = FeeType.CLIENT_FEE,
                SizeType = FeeSizeType.ABSOLUTE
            };
        }

        public double GetAmountWithFee(double initialAmount, Domain.Fee fee)
        {
            return initialAmount > 0 ? initialAmount + fee.Size : initialAmount - fee.Size;
        }
    }
}