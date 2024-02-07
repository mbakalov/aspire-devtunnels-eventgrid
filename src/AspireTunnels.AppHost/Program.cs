var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.SampleWebApi>("samplewebapi");

builder.Build().Run();
