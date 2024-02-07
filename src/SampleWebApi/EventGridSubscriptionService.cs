using Azure.Core;
using Azure.ResourceManager.EventGrid.Models;
using Azure.ResourceManager.EventGrid;
using Azure.ResourceManager;
using Azure;
using Azure.Identity;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace SampleWebApi;

public class EventGridSubscriptionOptions
{
    public string? ResourceArmId { get; set; }

    public string? SubscriptionName { get; set; }

    public string? WebhookRelativeUrl { get; set; }

    public string? EndpointUrl { get; set; }

    public string[]? IncludedEventTypes { get; set; }
}

public class EventGridSubscriptionService : BackgroundService
{
    private readonly EventGridSubscriptionOptions _options;
    private readonly ILogger<EventGridSubscriptionService> _logger;

    private readonly TaskCompletionSource _appReady = new();

    public EventGridSubscriptionService(
        IOptions<EventGridSubscriptionOptions> options,
        IHostApplicationLifetime lifetime,
        ILogger<EventGridSubscriptionService> logger)
    {
        lifetime.ApplicationStarted.Register(_appReady.SetResult);

        _options = options.Value;
        _logger = logger;

        ArgumentException.ThrowIfNullOrEmpty(_options.ResourceArmId);
        ArgumentException.ThrowIfNullOrEmpty(_options.SubscriptionName);
        ArgumentException.ThrowIfNullOrEmpty(_options.WebhookRelativeUrl);
        ArgumentException.ThrowIfNullOrEmpty(_options.EndpointUrl);
        ArgumentNullException.ThrowIfNull(_options.IncludedEventTypes);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Trace.Assert(_options.ResourceArmId != null);
        Trace.Assert(_options.IncludedEventTypes != null);

        await _appReady.Task.ConfigureAwait(false);

        var uri = _options.EndpointUrl;
        if (uri != null)
        {
            _logger.LogInformation("Using uri {0} to subscribe", uri);
            _logger.LogInformation("Arm resource Id: {0}", _options.ResourceArmId);

            var egClient = new ArmClient(new DefaultAzureCredential());

            var resourceId = new ResourceIdentifier(_options.ResourceArmId);
            var subs = egClient.GetEventSubscriptions(resourceId);

            var filter = new EventSubscriptionFilter();
            foreach (var eventType in _options.IncludedEventTypes)
            {
                filter.IncludedEventTypes.Add(eventType);
            }

            var subData = new EventGridSubscriptionData()
            {
                EventDeliverySchema = EventDeliverySchema.EventGridSchema,
                Destination = new WebHookEventSubscriptionDestination()
                {
                    Endpoint = new Uri($"{uri}{_options.WebhookRelativeUrl}")
                },
                Filter = filter
            };

            _logger.LogInformation("Starting subscription creation...");
            await subs.CreateOrUpdateAsync(WaitUntil.Started, _options.SubscriptionName, subData);
            _logger.LogInformation("Started");
        }
        else
        {
            _logger.LogInformation("EndpointUrl is empty, unable to subscribe");
        }
    }
}
