using StockQuoteAlert.Observer;
using StockQuoteAlert.StateMachine;
using StockQuoteAlert.StateMachine.States;
using Xunit;

namespace StockQuoteAlert.Tests;

public class ObserverQuoteTests
{
    [Fact]
    public void MonitorPrice_WhenPriceEqualsSellPrice_TransitionsToSellAlert()
    {
        var context = new Context(new Neutral());

        ObserverQuote.MonitorPrice(context, 100, 100, 90);

        Assert.IsType<SellAlert>(context.CurrentState);
    }

    [Fact]
    public void MonitorPrice_WhenPriceIsAboveSellPrice_TransitionsToSellAlert()
    {
        var context = new Context(new Neutral());

        ObserverQuote.MonitorPrice(context, 110, 100, 90);

        Assert.IsType<SellAlert>(context.CurrentState);
    }

    [Fact]
    public void MonitorPrice_WhenPriceEqualsBuyPrice_TransitionsToBuyAlert()
    {
        var context = new Context(new Neutral());

        ObserverQuote.MonitorPrice(context, 90, 100, 90);

        Assert.IsType<BuyAlert>(context.CurrentState);
    }

    [Fact]
    public void MonitorPrice_WhenPriceIsBelowBuyPrice_TransitionsToBuyAlert()
    {
        var context = new Context(new Neutral());

        ObserverQuote.MonitorPrice(context, 80, 100, 90);

        Assert.IsType<BuyAlert>(context.CurrentState);
    }

    [Fact]
    public void MonitorPrice_WhenPriceIsBetweenThresholds_ReturnsNeutral()
    {
        var context = new Context(new Neutral());

        ObserverQuote.MonitorPrice(context, 95, 100, 90);

        Assert.IsType<Neutral>(context.CurrentState);
    }
}