using StockQuoteAlert.Domain;
using StockQuoteAlert.Clients;
using StockQuoteAlert.Clients.BRAPI;
using StockQuoteAlert.Monitor;
using StockQuoteAlert.Notifications;
using StockQuoteAlert.StateMachine;
using StockQuoteAlert.StateMachine.States;
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

    [Fact]
    public async Task EvaluateStateByAction_WhenQuoteIsBelowBuyPrice_TriggersBuyAlert()
    {
        var notifier = new FakeAlertNotifier();
        var context = new Context(new Neutral(), notifier, Array.Empty<string>());
        var monitor = new QuoteMonitor(
            60,
            new FakeQuoteProvider(new Quote { Symbol = "PETR4", Data = new QuoteData { RegularMarketPrice = 90 } }),
            context);

        var result = await monitor.EvaluateStateByAction("PETR4", 100, 95, CancellationToken.None);

        Assert.True(result);
        Assert.IsType<BuyAlert>(context.CurrentState);
        Assert.Equal(StockAction.Buy, Assert.Single(notifier.Actions));
    }

    [Fact]
    public async Task EvaluateStateByAction_WhenQuoteIsAboveSellPrice_TriggersSellAlert()
    {
        var notifier = new FakeAlertNotifier();
        var context = new Context(new Neutral(), notifier, Array.Empty<string>());
        var monitor = new QuoteMonitor(
            60,
            new FakeQuoteProvider(new Quote { Symbol = "PETR4", Data = new QuoteData { RegularMarketPrice = 110 } }),
            context);

        var result = await monitor.EvaluateStateByAction("PETR4", 100, 95, CancellationToken.None);

        Assert.True(result);
        Assert.IsType<SellAlert>(context.CurrentState);
        Assert.Equal(StockAction.Sell, Assert.Single(notifier.Actions));
    }

    [Fact]
    public async Task EvaluateStateByAction_WhenQuoteIsMissing_ReturnsFalse()
    {
        var notifier = new FakeAlertNotifier();
        var context = new Context(new Neutral(), notifier, Array.Empty<string>());
        var monitor = new QuoteMonitor(60, new FakeQuoteProvider(null), context);

        var result = await monitor.EvaluateStateByAction("PETR4", 100, 95, CancellationToken.None);

        Assert.False(result);
        Assert.IsType<Neutral>(context.CurrentState);
        Assert.Empty(notifier.Actions);
    }

    private sealed class FakeQuoteProvider : IQuoteProvider
    {
        private readonly Quote? _quote;

        public FakeQuoteProvider(Quote? quote)
        {
            _quote = quote;
        }

        public Task<Quote?> GetQuoteAsync(string ticker, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_quote);
        }
    }

    private sealed class FakeAlertNotifier : IAlertNotifier
    {
        public List<StockAction> Actions { get; } = new();

        public Task SendEmail(
            string[] recipientList,
            string ticker,
            StockAction action,
            decimal price)
        {
            Actions.Add(action);
            return Task.CompletedTask;
        }
    }

}