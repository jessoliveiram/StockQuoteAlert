using DotNetEnv;
using System.Globalization;
using StockQuoteAlert.Clients.BRAPI;

public static class Program
{
    public static async Task Main()
    {
        Console.Write("Enter ticker, sell price and buy price: ");
        var args = Console.ReadLine()?.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (args is null || args.Length != 3)
        {
            Console.WriteLine("Usage: <TICKER> <SELL_PRICE> <BUY_PRICE>");
            Console.WriteLine("Example: PETR4 22.67 22.59");
            return;
        }

        if (!decimal.TryParse(args[1], NumberStyles.Number, CultureInfo.InvariantCulture, out _) ||
            !decimal.TryParse(args[2], NumberStyles.Number, CultureInfo.InvariantCulture, out _))
        {
            Console.WriteLine("Selling and buying prices must be valid decimal numbers.");
            return;
        }

        Env.TraversePath().Load("local.env");
        var token = Environment.GetEnvironmentVariable("BRAPI_TOKEN");

        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("BRAPI_TOKEN not found in environment variables.");
            return;
        }

        var brapiClient = new BRAPIClient(token);
        var ticker = args[0].ToUpperInvariant();
        var quote = await brapiClient.GetQuoteAsync(ticker);

        if (quote is null)
        {
            Console.WriteLine("Quote not found.");
            return;
        }

        Console.WriteLine($"{quote.Symbol}: R$ {quote.RegularMarketPrice:F2}");
    }
}
