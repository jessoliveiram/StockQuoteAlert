using DotNetEnv;
using System.Globalization;
using StockQuoteAlert.Clients.BRAPI;
using StockQuoteAlert.Configuration;
using StockQuoteAlert.Monitor;
using StockQuoteAlert.StateMachine;
using StockQuoteAlert.StateMachine.States;
using StockQuoteAlert.Notifications;
using System.Net.Mail;
using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Write("Enter ticker, sell price and buy price: ");
            args = Console.ReadLine()?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        }

        if (args.Length != 3)
        {
            Console.WriteLine("Usage: <TICKER> <SELL_PRICE> <BUY_PRICE>");
            Console.WriteLine("Example: PETR4 22.67 22.59");
            return;
        }

        if (!decimal.TryParse(args[1], NumberStyles.Number, CultureInfo.InvariantCulture, out var sellPrice) ||
            !decimal.TryParse(args[2], NumberStyles.Number, CultureInfo.InvariantCulture, out var buyPrice))
        {
            Console.WriteLine("Selling and buying prices must be valid decimal numbers.");
            return;
        }

        Env.TraversePath().Load("local.env");

        var settings = AppSettings.Load();
        var recipientList = settings.EmailService.RecipientList.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var observerInterval = settings.Observer.IntervalSeconds;

        var builder = Host.CreateApplicationBuilder();
        var EMAIL_PASSWORD = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
        
        builder.Services
            .AddFluentEmail(settings.EmailService.Smtp.FromAddress, "Stock Quote Alert")
            .AddSmtpSender(() =>
            {
                var smtpClient = new SmtpClient
                {
                    Host = settings.EmailService.Smtp.Host,
                    Port = settings.EmailService.Smtp.Port,
                    EnableSsl = settings.EmailService.Smtp.EnableSsl
                };

                if (!string.IsNullOrWhiteSpace(EMAIL_PASSWORD))
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(
                        settings.EmailService.Smtp.FromAddress,
                        EMAIL_PASSWORD
                    );
                }

                return smtpClient;
            });

        using var host = builder.Build();
        var emailSender = host.Services.GetRequiredService<IFluentEmailFactory>();
        var emailService = new EmailService(emailSender);

        var brapiClient = new BRAPIClient();
        var ticker = args[0].ToUpperInvariant();

        var initialStateNeutral = new Neutral();
        var stockStateContext = new Context(initialStateNeutral, emailService, recipientList);
        var quoteMonitor = new QuoteMonitor(observerInterval, brapiClient, stockStateContext);
        await quoteMonitor.StartMonitoring(ticker, sellPrice, buyPrice);
    }
}
