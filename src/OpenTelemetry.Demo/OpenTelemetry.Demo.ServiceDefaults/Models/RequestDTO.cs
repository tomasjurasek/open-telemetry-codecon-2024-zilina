namespace OpenTelemetry.Demo.ServiceDefaults.Models;

public record class RequestDTO
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public DateTime CreatedAt { get; init; }
    public required string Status { get; init; }

}
