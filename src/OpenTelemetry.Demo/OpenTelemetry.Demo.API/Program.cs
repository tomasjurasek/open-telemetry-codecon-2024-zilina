using Microsoft.Extensions.Caching.Distributed;
using OpenTelemetry.Demo.ServiceDefaults.Models;
using MassTransit;
using OpenTelemetry.Demo.API.Database;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IRequestStorage, RequestStorage>();

builder.Services.AddMassTransit(x =>
{

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("messaging"));

        cfg.Message<RequestCreatedEvent>(e => e.SetEntityName("request-created"));
        cfg.Publish<RequestCreatedEvent>();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.MapPost("/requests", async (IBus bus, IRequestStorage storage, CreateRequestCommand command) =>
{
    await storage.StoreAsync(new RequestDTO { Id = command.Id, Name = command.Name, Description = command.Description, CreatedAt = command.CreatedAt, Status = "Waiting" });
    await bus.Publish(new RequestCreatedEvent(command.Id, command.Name, command.Description));

    return Results.Ok(command.Id);
})
.WithName("AddRequest");

app.MapPost("/requests/{id}/status/{status}", async (Guid id, string status, IRequestStorage storage) =>
{
    var request = await storage.GetAsync(id);
    request = request with { Status = status };
    await storage.StoreAsync(request);

    return Results.Ok();
})
.WithName("ChangeRequestStatus");


app.MapGet("/requests", (IRequestStorage storage) =>
{
    return storage.GetAsync();
})
.WithName("GetRequests");

app.Run();

