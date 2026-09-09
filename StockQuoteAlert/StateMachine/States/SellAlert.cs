namespace StockQuoteAlert.StateMachine.States;


class SellAlert : State
{
    public override void TriggerNeutral()
    {
        Console.WriteLine("SellAlert changes the state to Neutral.");
        _context.TransitionTo(new Neutral());
    }

    public override void TriggerBuyAlert()
    {
        Console.WriteLine("SellAlert changes the state to Buy Alert.");
        _context.TransitionTo(new BuyAlert());
    }
}