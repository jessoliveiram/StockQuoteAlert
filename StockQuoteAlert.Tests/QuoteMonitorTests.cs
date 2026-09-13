using StockQuoteAlert.Domain;
using StockQuoteAlert.Monitor;
using Xunit;

namespace StockQuoteAlert.Tests;

public class QuoteMonitorTests
{
    [Fact]
    public void CheckActionByPrice_WhenPriceEqualsSellPrice_ReturnsSellAlert()
    {
        var result = QuoteMonitor.EvaluateActionByPrice(100, 100, 90);

        Assert.Equal(StockAction.Sell, result);
    }

    [Fact]
    public void CheckActionByPrice_WhenPriceIsAboveSellPrice_ReturnsSellAlert()
    {
        var result = QuoteMonitor.EvaluateActionByPrice(110, 100, 90);

        Assert.Equal(StockAction.Sell, result);
    }

    [Fact]
    public void CheckActionByPrice_WhenPriceEqualsBuyPrice_ReturnsBuyAlert()
    {
        var result = QuoteMonitor.EvaluateActionByPrice(90, 100, 90);

        Assert.Equal(StockAction.Buy, result);
    }

    [Fact]
    public void CheckActionByPrice_WhenPriceIsBelowBuyPrice_ReturnsBuyAlert()
    {
        var result = QuoteMonitor.EvaluateActionByPrice(80, 100, 90);

        Assert.Equal(StockAction.Buy, result);
    }

    [Fact]
    public void CheckActionByPrice_WhenPriceIsBetweenThresholds_ReturnsNeutral()
    {
        var result = QuoteMonitor.EvaluateActionByPrice(95, 100, 90);

        Assert.Equal(StockAction.Neutral, result);
    }

}