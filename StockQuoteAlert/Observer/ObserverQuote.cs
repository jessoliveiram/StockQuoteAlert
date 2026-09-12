using StockQuoteAlert.StateMachine;
using StockQuoteAlert.Clients.BRAPI;
using PeriodicTimer = System.Threading.PeriodicTimer;
using StockQuoteAlert.StateMachine.States;

namespace StockQuoteAlert.Observer;

internal class ObserverQuote
{
    private readonly int _intervalSeconds;
    private readonly BRAPIClient _brapiClient;
    private readonly Context _stockStateContext;

    private const string BuyAlert = "you should buy the stock";
    private const string SellAlert = "you should sell the stock";
    private const string Neutral = "you should hold the stock";

    public ObserverQuote(int intervalSeconds, BRAPIClient brapiClient, Context stockStateContext)
    {
        _brapiClient = brapiClient;
        _intervalSeconds = intervalSeconds;
        _stockStateContext = stockStateContext;
    }

    public async Task ObserverPrice(string ticker, decimal sellPrice, decimal buyPrice)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(_intervalSeconds));

        while (await timer.WaitForNextTickAsync())
        {
            var quote = await _brapiClient.GetQuoteAsync(ticker);
            if (quote is null)
            {
                Console.WriteLine("Quote not found.");
                return;
            }

            var price = quote.RegularMarketPrice;
            var action = await CheckPrice(price, sellPrice, buyPrice);

            switch (action)
            {
                case BuyAlert:
                    _stockStateContext.TriggerBuyAlert(ticker, price);
                    Console.WriteLine(BuyAlert);
                    break;

                case SellAlert:
                    _stockStateContext.TriggerSellAlert(ticker, price);
                    Console.WriteLine(SellAlert);
                    break;

                default:
                    _stockStateContext.TriggerNeutral(ticker, price);
                    Console.WriteLine(Neutral);
                    break;
            }
        }
    }

    public async Task<string> CheckPrice(decimal price, decimal sellPrice, decimal buyPrice)
    {
        if (ShouldSell(sellPrice, price))
        {
            Console.WriteLine($"Price {price} is greater than or equal to sell price {sellPrice}");
            return SellAlert;
        }
        else if (ShouldBuy(buyPrice, price))
        {
            Console.WriteLine($"Price {price} is less than or equal to buy price {buyPrice}");
            return BuyAlert;
        }
        else
        {
            Console.WriteLine($"Price {price} is between sell price {sellPrice} and buy price {buyPrice}.");
            return Neutral;
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
