namespace StockQuoteAlert.StateMachine.States;


class BuyAlert : State
{
    public override void TriggerNeutral(string ticker, decimal price)
    {
        Console.WriteLine("BuyAlert changes the state to Neutral.");
        _context.TransitionTo(new Neutral());
    }

    public override void TriggerSellAlert(string ticker, decimal price)
    {
        Console.WriteLine("BuyAlert changes the state to Sell Alert.");
        _context.TransitionTo(new SellAlert());

        _ = Task.Run(async () =>
        {
            try
            {
                await _context.EmailService.SendEmail(_context.RecipientList, ticker, "sell", price);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao enviar e-mail em background: {ex.Message}");
            }
        });
    }
}