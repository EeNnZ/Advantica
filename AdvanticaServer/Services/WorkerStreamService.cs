using Grpc.Core;
using AdvanticaServer.Protos;

namespace AdvanticaServer.Services
{
    public class WorkerStreamService : WorkerIntegration.WorkerIntegrationBase
    {
        private readonly ILogger<WorkerStreamService> _logger;
        public WorkerStreamService(ILogger<WorkerStreamService> logger) 
        {
            _logger = logger;
        }

        public override async Task GetWorkerStream(EmptyMessage request, IServerStreamWriter<WorkerAction> responseStream, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

    }
}
