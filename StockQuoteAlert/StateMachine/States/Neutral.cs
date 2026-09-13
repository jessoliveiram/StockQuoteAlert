namespace StockQuoteAlert.StateMachine.States;

using StockQuoteAlert.Domain;


class Neutral : State
{
    public override Task TriggerNeutral(string ticker, decimal price)
    {
        return Task.CompletedTask;
    }

    public override async Task TriggerSellAlert(string ticker, decimal price)
    {
        Console.WriteLine("Neutral changes the state to Sell Alert.");
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

    public override async Task TriggerBuyAlert(string ticker, decimal price)
    {
        Console.WriteLine("Neutral changes the state to Buy Alert.");
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