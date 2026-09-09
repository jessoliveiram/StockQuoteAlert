namespace StockQuoteAlert.StateMachine;
class Context
{
    private State _state = null!;

    internal State CurrentState => _state;

    public Context(State state)
    {
        TransitionTo(state);
    }

    public void TransitionTo(State state)
    {
        ArgumentNullException.ThrowIfNull(state);

        _state = state;
        _state.SetContext(this);
    }

    public void TriggerSellAlert()
    {
        _state.TriggerSellAlert();
    }

    public void TriggerBuyAlert()
    {
        _state.TriggerBuyAlert();
    }

    public void TriggerNeutral()
    {
        _state.TriggerNeutral();
    }
}