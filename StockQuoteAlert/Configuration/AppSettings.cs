using System.Text.Json;

namespace StockQuoteAlert.Configuration;

internal sealed class AppSettings
{
    public ObserverSettings Observer { get; init; } = new();

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
