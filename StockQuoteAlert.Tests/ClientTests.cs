using System.Net;
using StockQuoteAlert.Clients.BRAPI;
using Xunit;

namespace StockQuoteAlert.Tests;

public class ClientTests
{
    [Fact]
    public async Task GetQuoteAsync_SendsExpectedRequestAndMapsQuote()
    {
        var handler = new StubHttpMessageHandler(
            """
            {
              "results": [
                {
                  "symbol": "B3SA3",
                  "longName": "B3 SA - Brasil, Bolsa, Balcao",
                  "currency": "BRL",
                  "regularMarketPrice": 17.27,
                  "regularMarketChangePercent": -2.1
                }
              ]
            }
            """);
        using var httpClient = new HttpClient(handler);
        var client = new BRAPIClient(httpClient);

        var quote = await client.GetQuoteAsync("B3SA3");

        Assert.NotNull(quote);
        Assert.Equal(HttpMethod.Get, handler.Request!.Method);
        Assert.Equal("https://brapi.dev/api/quote/B3SA3", handler.Request.RequestUri!.ToString());
        Assert.Equal("B3SA3", quote.Symbol);
        Assert.Equal("B3 SA - Brasil, Bolsa, Balcao", quote.LongName);
        Assert.Equal("BRL", quote.Currency);
        Assert.Equal(17.27m, quote.RegularMarketPrice);
        Assert.Equal(-2.1m, quote.RegularMarketChangePercent);
    }

    [Fact]
    public async Task GetQuoteAsync_ReturnsNullWhenResultsAreEmpty()
    {
        var client = new BRAPIClient(new HttpClient(
            new StubHttpMessageHandler("{ \"results\": [] }")));

        var quote = await client.GetQuoteAsync("B3SA3");

        Assert.Null(quote);
    }

    [Fact]
    public async Task GetQuoteAsync_ThrowsForUnsuccessfulResponse()
    {
        var client = new BRAPIClient(new HttpClient(new StubHttpMessageHandler(
            "{ \"error\": \"not found\" }", HttpStatusCode.NotFound)));

        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetQuoteAsync("B3SA3"));
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _responseBody;
        private readonly HttpStatusCode _statusCode;

        public StubHttpMessageHandler(
            string responseBody,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _responseBody = responseBody;
            _statusCode = statusCode;
        }

        public HttpRequestMessage? Request { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Request = request;
            return Task.FromResult(new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_responseBody)
            });
        }
    }
}