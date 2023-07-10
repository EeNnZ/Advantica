using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advantica.IntegrationSystem.Options
{
    public sealed class IntegrationServiceOptions
    {
        public string? Url { get; set; }
        public int MinimumInactiveTimePeriodMilliseconds { get; set; }
        public int MaximumInactiveTimePeriodMilliseconds { get; set; }
    }
}
