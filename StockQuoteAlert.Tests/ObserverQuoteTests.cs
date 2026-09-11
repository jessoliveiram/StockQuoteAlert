using StockQuoteAlert.Observer;
using StockQuoteAlert.Clients.BRAPI;
using StockQuoteAlert.StateMachine;
using StockQuoteAlert.StateMachine.States;
using Xunit;

namespace StockQuoteAlert.Tests;

public class ObserverQuoteTests
{
    [Fact]
    public async Task CheckPrice_WhenPriceEqualsSellPrice_ReturnsSellAlert()
    {
        var observer = CreateObserver();

        var result = await observer.CheckPrice(100, 100, 90);

        Assert.Equal("You should sell the stock.", result);
    }

    [Fact]
    public async Task CheckPrice_WhenPriceIsAboveSellPrice_ReturnsSellAlert()
    {
        var observer = CreateObserver();

        var result = await observer.CheckPrice(110, 100, 90);

        Assert.Equal("You should sell the stock.", result);
    }

    [Fact]
    public async Task CheckPrice_WhenPriceEqualsBuyPrice_ReturnsBuyAlert()
    {
        var observer = CreateObserver();

        var result = await observer.CheckPrice(90, 100, 90);

        Assert.Equal("You should buy the stock.", result);
    }

    [Fact]
    public async Task CheckPrice_WhenPriceIsBelowBuyPrice_ReturnsBuyAlert()
    {
        var observer = CreateObserver();

        var result = await observer.CheckPrice(80, 100, 90);

        Assert.Equal("You should buy the stock.", result);
    }

    [Fact]
    public async Task CheckPrice_WhenPriceIsBetweenThresholds_ReturnsNeutral()
    {
        var observer = CreateObserver();

        var result = await observer.CheckPrice(95, 100, 90);

        Assert.Equal("You should hold the stock.", result);
    }

    private static ObserverQuote CreateObserver()
    {
        return new ObserverQuote(
            60,
            new BRAPIClient(new HttpClient()),
            new Context(new Neutral(), null!, Array.Empty<string>()));
    }
}