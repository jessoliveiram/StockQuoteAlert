namespace StockQuoteAlert.StateMachine;

abstract class State
{
    protected Context _context = null!;

    public void SetContext(Context context)
    {
        _context = context;
    }

    public virtual void TriggerSellAlert(string ticker, decimal price){}

    public virtual void TriggerBuyAlert(string ticker, decimal price){}

    public virtual void TriggerNeutral(string ticker, decimal price){}
}
