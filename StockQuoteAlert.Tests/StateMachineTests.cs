using StockQuoteAlert.StateMachine;
using StockQuoteAlert.StateMachine.States;
using Xunit;

namespace StockQuoteAlert.Tests;

public class StateMachineTests
{
    public static TheoryData<string, Type> InitialStates => new()
    {
        { "Neutral", typeof(Neutral) },
        { "BuyAlert", typeof(BuyAlert) },
        { "SellAlert", typeof(SellAlert) }
    };

    public static TheoryData<string, string, Type> Transitions => new()
    {
        { "Neutral", "SellAlert", typeof(SellAlert) },
        { "Neutral", "BuyAlert", typeof(BuyAlert) },
        { "BuyAlert", "Neutral", typeof(Neutral) },
        { "BuyAlert", "SellAlert", typeof(SellAlert) },
        { "SellAlert", "Neutral", typeof(Neutral) },
        { "SellAlert", "BuyAlert", typeof(BuyAlert) }
    };

    public static TheoryData<string, string> NoOpTriggers => new()
    {
        { "Neutral", "Neutral" },
        { "BuyAlert", "BuyAlert" },
        { "SellAlert", "SellAlert" }
    };

    [Theory]
    [MemberData(nameof(InitialStates))]
    public void Constructor_SetsInitialState(string initialStateName, Type expectedStateType)
    {
        var context = new Context(CreateState(initialStateName));

        Assert.IsType(expectedStateType, context.CurrentState);
    }

    [Theory]
    [MemberData(nameof(Transitions))]
    public void Trigger_TransitionsToExpectedState(
        string initialStateName,
        string triggerName,
        Type expectedStateType)
    {
        var context = new Context(CreateState(initialStateName));

        Trigger(context, triggerName);

        Assert.IsType(expectedStateType, context.CurrentState);
    }

    [Theory]
    [MemberData(nameof(NoOpTriggers))]
    public void UnsupportedTrigger_PreservesCurrentState(
        string initialStateName,
        string triggerName)
    {
        var context = new Context(CreateState(initialStateName));
        var stateBeforeTrigger = context.CurrentState;

        Trigger(context, triggerName);

        Assert.Same(stateBeforeTrigger, context.CurrentState);
    }

    private static State CreateState(string stateName) => stateName switch
    {
        "Neutral" => new Neutral(),
        "BuyAlert" => new BuyAlert(),
        "SellAlert" => new SellAlert(),
        _ => throw new ArgumentOutOfRangeException(nameof(stateName), stateName, null)
    };

    private static void Trigger(Context context, string triggerName)
    {
        switch (triggerName)
        {
            case "SellAlert":
                context.TriggerSellAlert();
                break;
            case "BuyAlert":
                context.TriggerBuyAlert();
                break;
            case "Neutral":
                context.TriggerNeutral();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(triggerName), triggerName, null);
        }
    }
}
