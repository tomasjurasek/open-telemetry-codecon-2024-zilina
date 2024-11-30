using MassTransit;
using OpenTelemetry.Demo.Worker.Consumers;
using OpenTelemetry.Demo.ServiceDefaults.Clients;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient<ApiClient>(c => {

    c.BaseAddress = new("https+http://api");
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<RequestCreatedEventConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("messaging"));
        cfg.ReceiveEndpoint("worker-consumer", c =>
        {
            c.Consumer(() => new RequestCreatedEventConsumer(context.GetService<ApiClient>()));

            c.Bind("request-created");
        });
    });
});

var host = builder.Build();
host.Run();
