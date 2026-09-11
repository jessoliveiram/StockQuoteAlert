using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using StockQuoteAlert.Notifications;
using Xunit;

namespace StockQuoteAlert.Tests;

public class EmailServiceTests
{
    [Fact]
    public async Task SendEmail_WhenActionIsBuy_RendersBuyTemplate()
    {
        var sender = await SendEmail("buy");

        var sentEmail = Assert.Single(sender.Messages);
        Assert.Equal("example@example.com", sentEmail.ToAddresses[0].EmailAddress);
        Assert.Equal("buy alert for PETR4", sentEmail.Subject);
        Assert.Contains($"It's time to buy PETR4 at {38.42m:F2}.", sentEmail.Body);
    }

    [Fact]
    public async Task SendEmail_WhenActionIsSell_RendersSellTemplate()
    {
        var sender = await SendEmail("sell");

        var sentEmail = Assert.Single(sender.Messages);
        Assert.Equal("example@example.com", sentEmail.ToAddresses[0].EmailAddress);
        Assert.Equal("sell alert for PETR4", sentEmail.Subject);
        Assert.Contains($"It's time to sell PETR4 at {38.42m:F2}.", sentEmail.Body);
    }

    private static async Task<CapturingSender> SendEmail(string action)
    {
        var sender = new CapturingSender();
        var factory = new CapturingEmailFactory(sender);
        var service = new EmailService(factory);
        await service.SendEmail(
            new List<string> { "example@example.com" },
            "PETR4",
            action,
            38.42m);

        return sender;
    }

    private sealed class CapturingEmailFactory : IFluentEmailFactory
    {
        private readonly CapturingSender _sender;

        public CapturingEmailFactory(CapturingSender sender)
        {
            _sender = sender;
        }

        public IFluentEmail Create()
        {
            return new Email("alerts@example.com", "Stock Quote Alert")
            {
                Sender = _sender
            };
        }
    }

    private sealed class CapturingSender : ISender
    {
        public List<EmailData> Messages { get; } = new();

        public SendResponse Send(IFluentEmail email, CancellationToken? cancellationToken = null)
        {
            Messages.Add(email.Data);
            return new SendResponse();
        }

        public Task<SendResponse> SendAsync(
            IFluentEmail email,
            CancellationToken? cancellationToken = null)
        {
            Messages.Add(email.Data);
            return Task.FromResult(new SendResponse());
        }
    }
}