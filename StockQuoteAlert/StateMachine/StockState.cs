namespace StockQuoteAlert.StateMachine;

abstract class State
{
    protected Context _context = null!;

    public void SetContext(Context context)
    {
        _context = context;
    }

    public abstract Task TriggerSellAlert(string ticker, decimal price);

    public abstract Task TriggerBuyAlert(string ticker, decimal price);

    public abstract Task TriggerNeutral(string ticker, decimal price);
}
