using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetEnv;

public class Quote
{
    public string Symbol { get; set; }
    public string ShortName { get; set; }
    public decimal RegularMarketPrice { get; set; }
    public decimal RegularMarketChangePercent { get; set; }
    public string Currency { get; set; }
}

public class QuoteResponse
{
    public Quote[] Results { get; set; }
}

public class BrapiClient
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://brapi.dev/api";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public BrapiClient(string token)
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<Quote> GetQuoteAsync(string ticker)
    {
        var url = $"{BaseUrl}/quote/{ticker}";
        var response = await _httpClient.GetStringAsync(url);

        var data = JsonSerializer.Deserialize<QuoteResponse>(response, JsonOptions);
        return data?.Results?[0];
    }

    static async Task Main()
    {
        Env.TraversePath().Load("local.env");
        var token = Environment.GetEnvironmentVariable("BRAPI_TOKEN");

        if (token == null)
        {
            Console.WriteLine("BRAPI_TOKEN not found in environment variables.");
            return;
        }

        var client = new BrapiClient(token);
        var quote = await client.GetQuoteAsync("PETR4");
        
        if (quote == null)
        {
            Console.WriteLine("Quote not found.");
            return;
        }

        Console.WriteLine($"{quote.Symbol}: R$ {quote.RegularMarketPrice:F2}");
    }
}