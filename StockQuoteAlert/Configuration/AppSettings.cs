using System.Text.Json;

namespace StockQuoteAlert.Configuration;

internal sealed class AppSettings
{
    public ObserverSettings Observer { get; init; } = new();

    public EmailServiceSettings EmailService { get; init; } = new();

    public static AppSettings Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<AppSettings>(json)
            ?? throw new InvalidOperationException("Unable to load appsettings.json.");
    }
}

internal sealed class ObserverSettings
{
    public int IntervalSeconds { get; init; }
}

internal sealed class EmailServiceSettings
{
    public SmtpSettings Smtp { get; init; } = new();
    public string RecipientList { get; init; } = string.Empty;
}

internal sealed class SmtpSettings
{
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; }
    public bool EnableSsl { get; init; } = true;
    public string FromAddress { get; init; } = string.Empty;
}
