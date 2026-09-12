using DotNetEnv;
using System.Globalization;
using StockQuoteAlert.Clients.BRAPI;
using StockQuoteAlert.Configuration;
using StockQuoteAlert.Observer;
using StockQuoteAlert.StateMachine;
using StockQuoteAlert.StateMachine.States;
using StockQuoteAlert.Notifications;
using System.Net.Mail;
using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main()
    {
        Console.Write("Enter ticker, sell price and buy price: ");
        var args = Console.ReadLine()?.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (args is null || args.Length != 3)
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
        var token = Environment.GetEnvironmentVariable("BRAPI_TOKEN");

        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("BRAPI_TOKEN not found in environment variables.");
            return;
        }

        var settings = AppSettings.Load();
        var recipientList = settings.EmailService.RecipientList.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var observerInterval = settings.Observer.IntervalSeconds;

        var builder = Host.CreateApplicationBuilder();
        builder.Services
            .AddFluentEmail(settings.EmailService.Smtp.FromAddress, "Stock Quote Alert")
            .AddSmtpSender(
                settings.EmailService.Smtp.Host, 
                settings.EmailService.Smtp.Port,          
                settings.EmailService.Smtp.FromAddress,
                settings.EmailService.Smtp.ApiKey
            );

        using var host = builder.Build();
        var emailSender = host.Services.GetRequiredService<IFluentEmailFactory>();
        var emailService = new EmailService(emailSender);

        var brapiClient = new BRAPIClient(token);
        var ticker = args[0].ToUpperInvariant();

        var initialStateNeutral = new Neutral();
        var stockStateContext = new Context(initialStateNeutral, emailService, recipientList);
        var observerQuote = new ObserverQuote(observerInterval, brapiClient, stockStateContext);
        await observerQuote.ObserverPrice(ticker, sellPrice, buyPrice);
    }
}
