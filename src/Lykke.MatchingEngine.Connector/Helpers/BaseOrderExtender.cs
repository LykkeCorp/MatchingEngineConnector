using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.MatchingEngine.Connector.Models;

namespace Lykke.MatchingEngine.Connector.Helpers
{
    public static class BaseOrderExtender
    {
        public const string Buy = "buy";
        public const string Sell = "sell";

        public static OrderAction OrderAction(this IOrderBase orderBase)
        {
            return orderBase.Volume > 0 ? Abstractions.Models.OrderAction.Buy : Abstractions.Models.OrderAction.Sell;
        }

        public static OrderAction? GetOrderAction(string actionWord)
        {
            if (actionWord.ToLower() == Buy)
                return Abstractions.Models.OrderAction.Buy;
            if (actionWord.ToLower() == Sell)
                return Abstractions.Models.OrderAction.Sell;

            return null;
        }

        public static OrderAction ViceVersa(this OrderAction orderAction)
        {
            if (orderAction == Abstractions.Models.OrderAction.Buy)
                return Abstractions.Models.OrderAction.Sell;
            return Abstractions.Models.OrderAction.Buy;
        }
    }
}