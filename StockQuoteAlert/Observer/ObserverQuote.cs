using StockQuoteAlert.StateMachine;
using StockQuoteAlert.Clients.BRAPI;
using PeriodicTimer = System.Threading.PeriodicTimer;

namespace StockQuoteAlert.Observer;

internal class ObserverQuote
{
    public static async Task ObserverPrice(Context context, BRAPIClient brapiClient, string ticker, decimal sellPrice, decimal buyPrice)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(60));

        while (await timer.WaitForNextTickAsync())
        {
            var quote = await brapiClient.GetQuoteAsync(ticker);
            if (quote is null)
            {
                Console.WriteLine("Quote not found.");
                return;
            }

            var price = quote.RegularMarketPrice;
            var alert = CheckPrice(context, price, sellPrice, buyPrice);
            Console.WriteLine($"{quote.Symbol}: R$ {price:F2}. Current State: {alert}");
        }
    }

    public static string CheckPrice(Context context, decimal price, decimal sellPrice, decimal buyPrice)
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
