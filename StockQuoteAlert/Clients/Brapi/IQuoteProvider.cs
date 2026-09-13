using StockQuoteAlert.Clients.BRAPI;

namespace StockQuoteAlert.Clients;

public interface IQuoteProvider
{
    Task<Quote?> GetQuoteAsync(string ticker, CancellationToken cancellationToken = default);
}
