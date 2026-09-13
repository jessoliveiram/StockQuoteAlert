using StockQuoteAlert.Notifications;

namespace StockQuoteAlert.StateMachine;
class Context
{
    private State _state = null!;
    private readonly EmailService _emailService;
    private readonly string[] _recipientList;

    internal State CurrentState => _state;

    public Context(State state, EmailService emailService, string[] recipientList)
    {
        _emailService = emailService;
        _recipientList = recipientList;
        TransitionTo(state);
    }

    public void TransitionTo(State state)
    {
        ArgumentNullException.ThrowIfNull(state);

        _state = state;
        _state.SetContext(this);
    }

    public EmailService EmailService => _emailService;
    public string[] RecipientList => _recipientList;


    public Task TriggerSellAlert(string ticker, decimal price)
    {
        return _state.TriggerSellAlert(ticker, price);
    }

    public Task TriggerBuyAlert(string ticker, decimal price)
    {
        return _state.TriggerBuyAlert(ticker, price);
    }

    public Task TriggerNeutral(string ticker, decimal price)
    {
        return _state.TriggerNeutral(ticker, price);
    }
}