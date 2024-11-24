namespace OpenTelemetry.Demo.ServiceDefaults.Models;

public record class CreateRequestCommand
{

    public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

