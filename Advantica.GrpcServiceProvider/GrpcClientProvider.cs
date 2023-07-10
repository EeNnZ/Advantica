using Grpc.Net.Client;
using Advantica.GrpcServiceProvider.Protos;
using Google.Protobuf.WellKnownTypes;

namespace Advantica.GrpcServiceProvider
{
    public class GrpcClientProvider : IDisposable
    {
        private readonly string _defaultUrl = "http://localhost:5249";

        private WorkerIntegration.WorkerIntegrationClient _workerIntegrationClient = null!;

        public string ServiceUrl { get; } = null!;

        public Lazy<GrpcChannel> DefaultRpcChannel { get; private set; }

        public GrpcClientProvider()
        {
            ServiceUrl = _defaultUrl;
            DefaultRpcChannel = new Lazy<GrpcChannel>(GrpcChannel.ForAddress(ServiceUrl));
        }
        public GrpcClientProvider(string serviceUrl)
        {
            ServiceUrl = UrlValid(serviceUrl) ? serviceUrl : _defaultUrl;
            DefaultRpcChannel = new Lazy<GrpcChannel>(GrpcChannel.ForAddress(ServiceUrl));
        }

        public WorkerIntegration.WorkerIntegrationClient GetWorkerIntegrationClient()
        {
            return _workerIntegrationClient ??= new WorkerIntegration.WorkerIntegrationClient(DefaultRpcChannel.Value);
        }

        private bool UrlValid(string value)
        {
            bool valid = Uri.TryCreate(value, UriKind.Absolute, out Uri? uriResult) && 
                                      (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return valid;
        }

        #region IDisposable
        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (DefaultRpcChannel.IsValueCreated)
                    {
                        DefaultRpcChannel.Value.Dispose();
                    }
                }
                _disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}