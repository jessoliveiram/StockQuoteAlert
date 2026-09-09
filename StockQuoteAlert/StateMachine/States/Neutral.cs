namespace StockQuoteAlert.StateMachine.States;


class Neutral : State
{
    public override void TriggerSellAlert()
    {
        Console.WriteLine("Neutral changes the state to Sell Alert.");
        _context.TransitionTo(new SellAlert());
    }

    public override void TriggerBuyAlert()
    {
        Console.WriteLine("Neutral changes the state to Buy Alert.");
        _context.TransitionTo(new BuyAlert());
    }
}