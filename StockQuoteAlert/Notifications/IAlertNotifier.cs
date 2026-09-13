using StockQuoteAlert.Domain;

namespace StockQuoteAlert.Notifications;

public interface IAlertNotifier
{
    Task SendEmail(
        string[] recipientList,
        string ticker,
        StockAction action,
        decimal price);
}
