using MassTransit;
using OpenTelemetry.Demo.ServiceDefaults.Clients;
using OpenTelemetry.Demo.ServiceDefaults.Models;

namespace OpenTelemetry.Demo.Worker.Consumers;

public class RequestCreatedEventConsumer(ApiClient apiClient) : IConsumer<RequestCreatedEvent>
{
    public async Task Consume(ConsumeContext<RequestCreatedEvent> context)
    {
        await SomeLogic();
        await ChangeStatusAsync(context.Message);
    }

    private static async Task SomeLogic()
    {
        await Task.Delay(Random.Shared.Next(1000, 3000));
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
