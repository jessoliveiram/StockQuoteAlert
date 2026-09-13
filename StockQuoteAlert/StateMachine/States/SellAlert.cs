namespace StockQuoteAlert.StateMachine.States;

using StockQuoteAlert.Domain;


class SellAlert : State
{
    public override void TriggerNeutral(string ticker, decimal price)
    {
        Console.WriteLine("SellAlert changes the state to Neutral.");
        _context.TransitionTo(new Neutral());
    }

    public override void TriggerBuyAlert(string ticker, decimal price)
    {
        Console.WriteLine("SellAlert changes the state to Buy Alert.");
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