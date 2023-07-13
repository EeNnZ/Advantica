using Advantica.GrpcServiceProvider;

namespace Advantica.IntegrationSystem.Options
{
    public sealed class IntegrationServiceOptions : IOptions
    {
        private string _url = "";
        public string Url
        {
            get => _url;
            set => _url = value ?? "";
        }
        public int MinimumInactiveTimePeriodMilliseconds { get; set; }
        public int MaximumInactiveTimePeriodMilliseconds { get; set; }
    }
}
