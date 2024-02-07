using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using SampleWebApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<EventGridSubscriptionOptions>().Configure(options =>
{
    // Populated by init.ps1
    options.ResourceArmId = Environment.GetEnvironmentVariable("ACS_RESOURCE_ARM_ID");

    // To identify subscription in Azure portal
    options.SubscriptionName = $"{Environment.GetEnvironmentVariable("COMPUTERNAME") ?? "unknown"}-aspire-sample";

    // Corresponds to MapPost below
    options.WebhookRelativeUrl = "events";
    options.EndpointUrl = Environment.GetEnvironmentVariable("EndpointUrl");

    // Subscribe to these Event Grid events as an example
    options.IncludedEventTypes = ["Microsoft.Communication.ChatMessageReceivedInThread"];
});

// Hosted service that waits until app is started and then tries to self-subscribe to
// EventGrid events
builder.Services.AddHostedService<EventGridSubscriptionService>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/events", (EventGridEvent[] events) =>
{
    app.Logger.LogInformation("Got /events callback");

    foreach (var @event in events)
    {
        if (@event.TryGetSystemEventData(out object eventData))
        {
            if (eventData is SubscriptionValidationEventData subscriptionValidationEventData)
            {
                var responseData = new SubscriptionValidationResponse
                {
                    ValidationResponse = subscriptionValidationEventData.ValidationCode
                };

                app.Logger.LogInformation("We subscribed successfully");

                return Results.Ok(responseData);
            }
        }
        
        // Handle other events
    }

    return Results.Ok();
});

app.Run();

