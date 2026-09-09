namespace StockQuoteAlert.Clients.BRAPI;

public class Quote
{
    public string Symbol { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public decimal RegularMarketPrice { get; set; }
    public decimal RegularMarketChangePercent { get; set; }
    public string? Currency { get; set; }
}

public class QuoteResponse
{
    public Quote[]? Results { get; set; }
}