using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class Extensions
{
    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.ConfigureOpenTelemetry();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        return builder;
    }

    public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        // Logs
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });


        builder.Services.AddOpenTelemetry()
            .UseOtlpExporter() // Default from OTEL_EXPORTER_OTLP_ENDPOINT
            // Metrics
            .WithMetrics(metrics => 
            {
                metrics.AddMeter(builder.Environment.ApplicationName, InstrumentationOptions.MeterName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            // Tracing
            .WithTracing(tracing => 
            {
                tracing.AddSource(builder.Environment.ApplicationName, DiagnosticHeaders.DefaultListenerName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        return builder;
    }
}
