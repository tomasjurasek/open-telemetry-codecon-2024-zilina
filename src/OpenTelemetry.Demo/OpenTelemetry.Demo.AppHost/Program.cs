var builder = DistributedApplication.CreateBuilder(args);

var messaging = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin()
    .WithDataVolume();

var api = builder.AddProject<Projects.OpenTelemetry_Demo_API>("api")
    .WithReference(messaging)
    .WaitFor(messaging);

var frontend = builder.AddProject<Projects.OpenTelemetry_Demo_Frontend>("frontend")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

var worker = builder.AddProject<Projects.OpenTelemetry_Demo_Worker>("worker")
    .WithReference(messaging)
    .WaitFor(messaging)
    .WithReference(api);



builder.Build().Run();
