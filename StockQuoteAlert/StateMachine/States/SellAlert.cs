namespace StockQuoteAlert.StateMachine.States;

using StockQuoteAlert.Domain;


class SellAlert : State
{
    public override Task TriggerSellAlert(string ticker, decimal price)
    {
        return Task.CompletedTask;
    }

    public override Task TriggerNeutral(string ticker, decimal price)
    {
        Console.WriteLine("SellAlert changes the state to Neutral.");
        _context.TransitionTo(new Neutral());
        return Task.CompletedTask;
    }

    public override async Task TriggerBuyAlert(string ticker, decimal price)
    {
        Console.WriteLine("SellAlert changes the state to Buy Alert.");
        _context.TransitionTo(new BuyAlert());

        try
        {
            await _context.AlertNotifier.SendEmail(_context.RecipientList, ticker, StockAction.Buy, price);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Falha ao enviar e-mail: {ex.Message}");
        }

    }
}