namespace StockQuoteAlert.StateMachine;

abstract class State
{
    protected Context _context = null!;

    public void SetContext(Context context)
    {
        _context = context;
    }

    public virtual void TriggerSellAlert(){}

    public virtual void TriggerBuyAlert(){}

    public virtual void TriggerNeutral(){}
}
