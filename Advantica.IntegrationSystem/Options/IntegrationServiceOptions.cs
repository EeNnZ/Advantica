using Advantica.GrpcServiceProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

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
