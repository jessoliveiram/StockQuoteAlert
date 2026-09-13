using StockQuoteAlert.StateMachine;
using StockQuoteAlert.Clients.BRAPI;
using StockQuoteAlert.Domain;
using PeriodicTimer = System.Threading.PeriodicTimer;
using StockQuoteAlert.StateMachine.States;

namespace StockQuoteAlert.Observer;

internal class ObserverQuote
{
    private readonly int _intervalSeconds;
    private readonly BRAPIClient _brapiClient;
    private readonly Context _stockStateContext;


    public ObserverQuote(int intervalSeconds, BRAPIClient brapiClient, Context stockStateContext)
    {
        _brapiClient = brapiClient;
        _intervalSeconds = intervalSeconds;
        _stockStateContext = stockStateContext;
    }

    public async Task ObserverPrice(string ticker, decimal sellPrice, decimal buyPrice, CancellationToken cancellationToken = default)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_intervalSeconds));

        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            var quote = await _brapiClient.GetQuoteAsync(ticker);
            if (quote is null)
            {
                Console.WriteLine("Quote not found.");
                return;
            }

            var price = quote.RegularMarketPrice;
            var action = CheckPrice(price, sellPrice, buyPrice);

            switch (action)
            {
                case StockAction.Buy:
                    await _stockStateContext.TriggerBuyAlert(ticker, price);
                    Console.WriteLine(action.ToConsoleMessage());
                    break;

                case StockAction.Sell:
                    await _stockStateContext.TriggerSellAlert(ticker, price);
                    Console.WriteLine(action.ToConsoleMessage());
                    break;

                default:
                    await _stockStateContext.TriggerNeutral(ticker, price);
                    Console.WriteLine(action.ToConsoleMessage());
                    break;
            }
        }
    }

    public StockAction CheckPrice(decimal price, decimal sellPrice, decimal buyPrice)
    {
        if (ShouldSell(sellPrice, price))
        {
            Console.WriteLine($"Price {price} is greater than or equal to sell price {sellPrice}");
            return StockAction.Sell;
        }
        else if (ShouldBuy(buyPrice, price))
        {
            Console.WriteLine($"Price {price} is less than or equal to buy price {buyPrice}");
            return StockAction.Buy;
        }
        else
        {
            Console.WriteLine($"Price {price} is between sell price {sellPrice} and buy price {buyPrice}.");
            return StockAction.Neutral;
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
