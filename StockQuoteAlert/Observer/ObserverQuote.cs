using StockQuoteAlert.StateMachine;

namespace StockQuoteAlert.Observer;

internal class ObserverQuote
{
    public static string MonitorPrice(Context context, decimal price, decimal sellPrice, decimal buyPrice)
    {
        if (ShouldSell(sellPrice, price))
        {
            Console.WriteLine($"Price {price} is greater than or equal to sell price {sellPrice}");
            context.TriggerSellAlert();
            return context.CurrentState.GetType().Name; 
        }
        else if (ShouldBuy(buyPrice, price))
        {
            Console.WriteLine($"Price {price} is less than or equal to buy price {buyPrice}");
            context.TriggerBuyAlert();
            return context.CurrentState.GetType().Name;
        }
        else
        {
            Console.WriteLine($"Price {price} is between sell price {sellPrice} and buy price {buyPrice}.");
            context.TriggerNeutral();
            return context.CurrentState.GetType().Name;
        }
    }

    private static bool ShouldSell(decimal sellPrice, decimal price)
    {
        return price >= sellPrice;
    }

    private static bool ShouldBuy(decimal buyPrice, decimal price)
    {
        return price <= buyPrice;
    }
}
