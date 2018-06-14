using Common;
using Lykke.MatchingEngine.Connector.Models.Api;
using Lykke.MatchingEngine.Connector.Models.Common;
using Lykke.MatchingEngine.Connector.Models.Me;
using System;

namespace Lykke.MatchingEngine.Connector
{
    internal static class FeeExtensions
    {
        internal static FeeType GetFeeType(this double feeSize)
        {
            return feeSize > 0.0 ? FeeType.CLIENT_FEE : FeeType.NO_FEE;
        }

        internal static FeeModel GenerateTransferFee(
            this FeeModel feeModel,
            double amount,
            int accuracy)
        {
            var feeAbsolute = Math.Round(amount * feeModel.Size, 15).TruncateDecimalPlaces(accuracy, true);

            if (Math.Abs(feeAbsolute) > double.Epsilon)
                return new FeeModel
                {
                    Type = FeeType.CLIENT_FEE,
                    Size = feeAbsolute,
                    SourceClientId = null,
                    TargetClientId = feeModel.TargetClientId,
                    SizeType = FeeSizeType.ABSOLUTE,
                };

            return null;
        }

        internal static double CalculateAmountWithFee(
            this FeeModel feeModel,
            double sourceAmount,
            int accuracy)
        {
            if (feeModel.Type == FeeType.EXTERNAL_FEE)
                return sourceAmount;

            var amount = 0d;
            switch (feeModel.SizeType)
            {
                case FeeSizeType.ABSOLUTE:
                    amount = sourceAmount > 0 ? sourceAmount + feeModel.Size : sourceAmount - feeModel.Size;
                    break;
                case FeeSizeType.PERCENTAGE:
                    amount = Math.Round(Math.Abs(sourceAmount) * (1 + feeModel.Size), 15);
                    break;
            }
            return amount.TruncateDecimalPlaces(accuracy, true);
        }

        internal static FeeModel GenerateCashInOutFee(
            double amount,
            int accuracy,
            string clientId,
            double feeSize = 0d,
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
                        feeAbsolute = Math.Round(Math.Abs(amount) * feeSize, 15);
                        break;
                }

                feeAbsolute = feeAbsolute.TruncateDecimalPlaces(accuracy, true);
            }

            if (Math.Abs(feeAbsolute) > 0)
                return new FeeModel
                {
                    Type = feeSize.GetFeeType(),
                    Size = feeAbsolute,
                    SourceClientId = null,
                    TargetClientId = clientId,
                    SizeType = FeeSizeType.ABSOLUTE,
                };

            return null;
        }

        internal static FeeContract ToMeModel(this FeeModel src)
        {
            return new FeeContract
            {
                Size = src.Size,
                SizeType = (int)src.SizeType,
                SourceClientId = src.SourceClientId,
                TargetClientId = src.TargetClientId,
                Type = (int)src.Type
            };
        }
    }
}
