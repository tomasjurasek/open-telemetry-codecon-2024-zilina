using System.Diagnostics;

namespace OpenTelemetry.Demo.Worker
{
    public static class ActivitySourceProvider
    {
        public static ActivitySource ActivitySource => new ActivitySource("OpenTelemetry.Demo.Worker");
    }
}
