using OpenTelemetry.Demo.ServiceDefaults.Models;
using System.Net.Http.Json;

namespace OpenTelemetry.Demo.ServiceDefaults.Clients;

public class ApiClient(HttpClient httpClient)
{
    public async Task CreateRequestAsync(CreateRequestCommand command, CancellationToken cancellationToken = default)
    {
        var result = await httpClient.PostAsJsonAsync("/requests", command, cancellationToken);
        result.EnsureSuccessStatusCode();
    }

    public async Task ChangeStatusAsync(Guid id, string status, CancellationToken cancellationToken = default)
    {
        var result = await httpClient.PostAsync($"/requests/{id}/status/{status}", null, cancellationToken: cancellationToken);
        result.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyCollection<RequestDTO>> GetRequestsAsync(CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<IReadOnlyCollection<RequestDTO>>("/requests", cancellationToken);
    }
}
