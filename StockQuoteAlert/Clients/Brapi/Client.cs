using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using StockQuoteAlert.Clients;

namespace StockQuoteAlert.Clients.BRAPI;

public class BRAPIClient : IQuoteProvider
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://brapi.dev";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public BRAPIClient(): this(CreateHttpClient()){}

    public BRAPIClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private static HttpClient CreateHttpClient()
    {
        var httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        return httpClient;
    }

    public async Task<Quote?> GetQuoteAsync(string ticker, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/api/v2/stocks/quote?symbols={ticker}";
        var response = await _httpClient.GetStringAsync(url, ct);

        var data = JsonSerializer.Deserialize<QuoteResponse>(response, JsonOptions);
        var quote = data?.Results?.FirstOrDefault();
        
        if (quote?.Data == null || quote.Data.RegularMarketPrice == 0)
        {
            return null;
        }
        
        return quote;
    }
}