var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AntSK>("antsk");

builder.Build().Run();
