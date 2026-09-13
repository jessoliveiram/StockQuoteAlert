using StockQuoteAlert.StateMachine;
using StockQuoteAlert.Clients.BRAPI;
using StockQuoteAlert.Clients;
using StockQuoteAlert.Domain;
using PeriodicTimer = System.Threading.PeriodicTimer;
using StockQuoteAlert.StateMachine.States;

namespace StockQuoteAlert.Monitor;

internal class QuoteMonitor
{
    private readonly int _intervalSeconds;
    private readonly IQuoteProvider _quoteProvider;
    private readonly Context _stockStateContext;


    public QuoteMonitor(int intervalSeconds, IQuoteProvider quoteProvider, Context stockStateContext)
    {
        _quoteProvider = quoteProvider;
        _intervalSeconds = intervalSeconds;
        _stockStateContext = stockStateContext;
    }

    public async Task StartMonitoring(string ticker, decimal sellPrice, decimal buyPrice, CancellationToken cancellationToken = default)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_intervalSeconds));

        await EvaluateStateByAction(ticker, sellPrice, buyPrice, cancellationToken);

        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            await EvaluateStateByAction(ticker, sellPrice, buyPrice, cancellationToken);
        }
    }

    public async Task<bool> EvaluateStateByAction(string ticker, decimal sellPrice, decimal buyPrice, CancellationToken cancellationToken)
    {
        var quote = await _quoteProvider.GetQuoteAsync(ticker, cancellationToken);
        if (quote is null)
        {
            Console.WriteLine("Quote not found.");
            return false;
        }

        var price = quote.Data.RegularMarketPrice;
        var action = EvaluateActionByPrice(price, sellPrice, buyPrice);

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

        return true;
    }

    public static StockAction EvaluateActionByPrice(decimal price, decimal sellPrice, decimal buyPrice)
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
