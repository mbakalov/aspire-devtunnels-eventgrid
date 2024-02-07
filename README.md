# aspire-devtunnels-eventgrid

Experiments with APIs for local dev env with dotnet Aspire, Azure EventGrid, and Microsoft Dev tunnels.

## Motivation

In many Azure resources, such as [Azure Communication Services](https://learn.microsoft.com/azure/communication-services/overview),
a common workflow is to use EventGrid to subscribe to events via e.g. webhooks.

This repo explores possible ways to organize nice development experience for
EventGrid subscriptions with Aspire and how to have webhooks working locally
with [Dev tunnels](https://learn.microsoft.com/azure/developer/dev-tunnels/).

## Pre-requisites

1. Install dev tunnels `winget install Microsoft.devtunnel`

1. Install az cli: `winget install -e --id Microsoft.AzureCLI`

1. Have an Azure Subscription where you can create resources

## Running the sample

1. Run `.\init.ps1`
  
    * This will use devtunnels CLI to create a tunnel called "aspire-tunnel" and
      expose local port 5099 (on which the SampleWebApi is hosted)

    * It will also create an Azure Communication Services resource just to be able
      to have an EventGrid instance and to test subscribing to some events.

    * The script will ask you to enter name of the Azure subscription you wish
      to deploy resources to.

    * It will create a resource group called `rg-aspire-devtunnels-test` and the
      Communication resource called `acs-aspire-devtunnels-test`.

    * Finally it will initalize environment variables required by the app:

    * `ConnectionStrings__ACS` - to add the ACS resource (even though it is not
      really used)

    * `ACS_RESOURCE_ARM_ID` - Azure id of the ACS resource to be passed to EventGrid's
      conrol plane SDK and subscribe.

1. Build and run `AspireTunnels.sln`.

    * This should start up devtunnels via Aspire host
    
    * It will also use the devtunnels SDK to find out the public uri that got assigned
      to the devtunnel.

    * This public uri is then passed via environment variable `EndpointUrl` to
      the SampleWebApi process

    * The SampleWebApi process uses a background service `EventGridSubscriptionService`
      to wait until the app is ready and then self-subscribe to EventGrid using the
      `EndpointUrl`.

If everything works OK you should see 

```
...
Got /events callback
We subscribed successfully
```

In the SampleWebApi logs.
