using System.Globalization;
using FluentEmail.Core;

namespace StockQuoteAlert.Notifications;

public sealed class EmailService
{
  private const string SubjectTemplate = "{{ Action }} alert for {{ Ticker }}";
  private const string BodyTemplate = "Hello! I have an update on your {{ticker}}. The current price is {{price}}. It is a good time to {{action}}. Don't miss this opportunity!";

  private readonly IFluentEmailFactory _fluentEmailFactory;

  public EmailService(IFluentEmailFactory fluentEmailFactory)
  {
    _fluentEmailFactory = fluentEmailFactory;
  }

  public async Task SendEmail(string[] recipientList, string ticker, string action, decimal price)
  {
    var subject = SubjectTemplate
        .Replace("{{ Action }}", action, StringComparison.Ordinal)
        .Replace("{{ Ticker }}", ticker, StringComparison.Ordinal);

    var body = BodyTemplate
      .Replace("{{action}}", action, StringComparison.Ordinal)
      .Replace("{{ticker}}", ticker, StringComparison.Ordinal)
      .Replace("{{price}}", price.ToString("F2", CultureInfo.InvariantCulture), StringComparison.Ordinal);
        
    foreach (var recipient in recipientList)
    {
        var response = await _fluentEmailFactory
            .Create()
            .To(recipient)
            .Subject(subject)
            .Body(body)
            .SendAsync();

        if (!response.Successful)
        {
            Console.WriteLine($"Failed to send email to {recipient}");
        }
    }
  }
}