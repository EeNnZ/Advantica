using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advantica.GrpcServiceProvider
{
    public interface IOptions
    {
        string Url { get; set; }
    }
}
