using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace StockQuoteAlert.Clients.BRAPI;

public class BRAPIClient
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://brapi.dev/api";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public BRAPIClient(string token): this(CreateHttpClient(token)){}

    public BRAPIClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private static HttpClient CreateHttpClient(string token)
    {
        var httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        return httpClient;
    }

    public async Task<Quote?> GetQuoteAsync(string ticker, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/quote/{ticker}";
        var response = await _httpClient.GetStringAsync(url, ct);

        var data = JsonSerializer.Deserialize<QuoteResponse>(response, JsonOptions);
        return data?.Results?.FirstOrDefault();
    }
}