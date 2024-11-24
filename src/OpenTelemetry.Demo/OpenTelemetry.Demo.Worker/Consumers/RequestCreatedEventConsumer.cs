using MassTransit;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Demo.ServiceDefaults.Clients;
using OpenTelemetry.Demo.ServiceDefaults.Models;

namespace OpenTelemetry.Demo.Worker.Consumers;

public class RequestCreatedEventConsumer(ApiClient apiClient) : IConsumer<RequestCreatedEvent>
{
    public async Task Consume(ConsumeContext<RequestCreatedEvent> context)
    {
        // Load something
        await Task.Delay(Random.Shared.Next(1000, 3000));
        await ChangeStatusAsync(context.Message);
    }

    private async Task ChangeStatusAsync(RequestCreatedEvent message)
    {
        using var activity = ActivitySourceProvider.ActivitySource.StartActivity(nameof(ChangeStatusAsync));
        if(message.Description.Contains("PLEASE"))
        {
            await apiClient.ChangeStatusAsync(message.Id, "Approved");
        }
        else
        {
            await apiClient.ChangeStatusAsync(message.Id, "Declined");
        }
        
    }
}
