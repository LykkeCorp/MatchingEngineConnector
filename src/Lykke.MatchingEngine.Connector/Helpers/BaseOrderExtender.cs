using Lykke.MatchingEngine.Connector.Models.Common;

namespace Lykke.MatchingEngine.Connector.Helpers
{
    public static class BaseOrderExtender
    {
        public const string Buy = "buy";
        public const string Sell = "sell";

        public static OrderAction? GetOrderAction(string actionWord)
        {
            if (actionWord.ToLower() == Buy)
                return Models.Common.OrderAction.Buy;
            if (actionWord.ToLower() == Sell)
                return Models.Common.OrderAction.Sell;

            return null;
        }

        public static OrderAction ViceVersa(this OrderAction orderAction)
        {
            if (orderAction == Models.Common.OrderAction.Buy)
                return Models.Common.OrderAction.Sell;
            return Models.Common.OrderAction.Buy;
        }
    }
}