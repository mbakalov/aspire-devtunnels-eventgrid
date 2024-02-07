using AspireExtensions;

var builder = DistributedApplication.CreateBuilder(args);

var acs = builder.AddAzureCommunicationServices("ACS");

// TODO: Use devtunnel sdk to create the tunnel here? Instead of a separate .ps1 script
// How to know at this point what port the samplewebapi will be bound to locally?
const ushort port = 5099;
var tunnelPort = builder.AddDevTunnel("aspire-tunnel").AddPort("samplewebapi-port", port);

// TODO: Should this be in the Resource?
// Whats the right model to perform resource "initialization"?
await tunnelPort.Resource.LoadPublicDevtunnelUriAsync();

builder.AddProject<Projects.SampleWebApi>("samplewebapi")
    .WithReference(acs)
    .WithReference(tunnelPort);

builder.Build().Run();
