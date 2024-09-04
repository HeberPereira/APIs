using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;

namespace hb29.API.Helpers
{
    public class ApplicationInsightsRoleNameInitialiser : ITelemetryInitializer
    {
        private string RoleName { get; }
        public ApplicationInsightsRoleNameInitialiser(IConfiguration configuration)
        {
            RoleName = configuration["ApplicationInsights:RoleName"];
        }
        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = RoleName;
        }
    }
}
