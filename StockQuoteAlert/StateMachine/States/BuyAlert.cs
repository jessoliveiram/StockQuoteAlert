namespace StockQuoteAlert.StateMachine.States;

using StockQuoteAlert.Domain;


class BuyAlert : State
{
    public override Task TriggerBuyAlert(string ticker, decimal price)
    {
        return Task.CompletedTask;
    }

    public override Task TriggerNeutral(string ticker, decimal price)
    {
        Console.WriteLine("BuyAlert changes the state to Neutral.");
        _context.TransitionTo(new Neutral());
        return Task.CompletedTask;
    }

    public override async Task TriggerSellAlert(string ticker, decimal price)
    {
        Console.WriteLine("BuyAlert changes the state to Sell Alert.");
        _context.TransitionTo(new SellAlert());

        try
        {
            await _context.AlertNotifier.SendEmail(_context.RecipientList, ticker, StockAction.Sell, price);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Falha ao enviar e-mail: {ex.Message}");
        }

    }
}