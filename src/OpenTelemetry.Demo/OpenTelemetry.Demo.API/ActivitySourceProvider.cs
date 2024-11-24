using System.Diagnostics;

namespace OpenTelemetry.Demo.API
{
    public static class ActivitySourceProvider
    {
        public static ActivitySource ActivitySource => new ActivitySource("OpenTelemetry.Demo.API");
    }
}
