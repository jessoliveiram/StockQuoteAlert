namespace StockQuoteAlert.StateMachine.States;


class BuyAlert : State
{
    public override void TriggerNeutral()
    {
        Console.WriteLine("BuyAlert changes the state to Neutral.");
        _context.TransitionTo(new Neutral());
    }

    public override void TriggerSellAlert()
    {
        Console.WriteLine("BuyAlert changes the state to Sell Alert.");
        _context.TransitionTo(new SellAlert());
    }
}