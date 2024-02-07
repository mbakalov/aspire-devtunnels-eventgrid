using AspireExtensions;

var builder = DistributedApplication.CreateBuilder(args);

var acs = builder.AddAzureCommunicationServices("ACS");

builder.AddProject<Projects.SampleWebApi>("samplewebapi")
    .WithReference(acs);

builder.Build().Run();
