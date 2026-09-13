namespace StockQuoteAlert.Domain;

public enum StockAction
{
    Neutral,
    Buy,
    Sell
}

public static class StockActionMessages
{
    public static string ToConsoleMessage(this StockAction action) => action switch
    {
        StockAction.Buy => "You should buy the stock!",
        StockAction.Sell => "You should sell the stock!",
        StockAction.Neutral => "You should hold the stock.",
        _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
    };

    public static string ToEmailAction(this StockAction action) => action switch
    {
        StockAction.Buy => "buy",
        StockAction.Sell => "sell",
        StockAction.Neutral => throw new ArgumentException("Neutral does not generate an email.", nameof(action)),
        _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
    };
}
