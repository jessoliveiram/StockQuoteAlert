namespace StockQuoteAlert.StateMachine.States;

using StockQuoteAlert.Domain;


class Neutral : State
{
    public override void TriggerSellAlert(string ticker, decimal price)
    {
        Console.WriteLine("Neutral changes the state to Sell Alert.");
        _context.TransitionTo(new SellAlert());

         _ = Task.Run(async () =>
        {
            try
            {
                await _context.EmailService.SendEmail(_context.RecipientList, ticker, StockAction.Sell, price);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao enviar e-mail em background: {ex.Message}");
            }
        });
    }

    public override void TriggerBuyAlert(string ticker, decimal price)
    {
        Console.WriteLine("Neutral changes the state to Buy Alert.");
        _context.TransitionTo(new BuyAlert());

         _ = Task.Run(async () =>
        {
            try
            {
                await _context.EmailService.SendEmail(_context.RecipientList, ticker, StockAction.Buy, price);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao enviar e-mail em background: {ex.Message}");
            }
        });
    }
}