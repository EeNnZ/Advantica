using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advantica.GrpcServiceProvider;
using Advantica.IntegrationSystem.Options;

using Timer = System.Timers.Timer;

namespace Advantica.IntegrationSystem.Services
{
    /// <summary>
    /// A service working independetly and modifies database in a random timestamps.
    /// </summary>
    internal class IntegrationService : IDisposable
    {
        private readonly GrpcClientProvider _grpcClientProvider;

        private string[] _firstNames;
        private string[] _lastNames;

        private Random _random;
        private Timer _timer;

        /// <summary>
        /// Returns internal timer.
        /// </summary>
        public Timer Timer => _timer;

        /// <summary>
        /// Determines when the next action will be fired.
        /// </summary>
        public double NextFire => _timer.Interval / 1000;

        /// <summary>
        /// Determines grpc service url and timespans for random generator.
        /// </summary>
        public IntegrationServiceOptions ServiceOptions { get; set; }

        /// <summary>
        /// Grpc client that service uses.
        /// </summary>
        public GrpcServiceProvider.Protos.WorkerIntegration.WorkerIntegrationClient GrpcClient { get; private set; }

        public IntegrationService(IntegrationServiceOptions serviceOptions)
        {
            ServiceOptions = serviceOptions;

            _grpcClientProvider = new GrpcClientProvider(ServiceOptions.Url);
            GrpcClient = _grpcClientProvider.GetWorkerIntegrationClient();

            _firstNames = File.ReadAllLines("Files\\first_names.txt");
            _lastNames = File.ReadAllLines("Files\\last_names.txt");

            _random = new Random();
            _timer = new Timer();

            _timer.Elapsed += TimerElapsed;
        }

        /// <summary>
        /// Chooses random worker from database then modifies it.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public async Task UpdateRandomWorker()
        {
            var data = GrpcClient.GetWorkerStream(new GrpcServiceProvider.Protos.EmptyMessage());

            var responseStream = data.ResponseStream;

            var workers = new List<GrpcServiceProvider.Protos.WorkerMessage>();
            while (await responseStream.MoveNext(new CancellationToken()))
            {
                var response = responseStream.Current;
                workers.Add(response.Worker);
            }

            //To prevent update nothing
            if (workers.Count < 5)
            {
                CreateNewWorker();
                return;
            }

            var randomWorker = workers[new Random().Next(workers.Count - 1)];

            if (!randomWorker.HasChildren)
            {
                randomWorker.HasChildren = true;
            }

            randomWorker.LastName = _lastNames[new Random().Next(_lastNames.Length - 1)];

            var workerActionPut = new GrpcServiceProvider.Protos.WorkerAction()
            {
                ActionType = GrpcServiceProvider.Protos.Action.Update,
                Worker = randomWorker
            };

            GrpcServiceProvider.Protos.WorkerAction result = GrpcClient.PutWorker(workerActionPut);

            Console.WriteLine($"Worker: {randomWorker.RowIdMessage.WorkerRowId} updated. New last name: {randomWorker.LastName}, HasChildren: {randomWorker.HasChildren}");
        }

        /// <summary>
        /// Creates a new worker which properties based on random.
        /// </summary>
        public void CreateNewWorker()
        {
            var random = new Random();

            var newWorker = new GrpcServiceProvider.Protos.WorkerAction()
            {
                ActionType = GrpcServiceProvider.Protos.Action.Create,
                Worker = new GrpcServiceProvider.Protos.WorkerMessage()
                {
                    FirstName = _firstNames[random.Next(_firstNames.Length - 1)],
                    MiddleName = _firstNames[random.Next(_firstNames.Length - 1)],
                    LastName = _lastNames[random.Next(_lastNames.Length - 1)],
                    Birthday = new DateTime(random.Next(1950, 2004), random.Next(1, 12), random.Next(1, 30)).ToBinary(),
                    HasChildren = random.Next(2) == 1,
                    //TODO: Gender api
                    Sex = (GrpcServiceProvider.Protos.Sex)random.Next(1,3),
                    RowIdMessage = new GrpcServiceProvider.Protos.WorkerRowIdMessage() { WorkerRowId = 0 }
                }
            };

            GrpcServiceProvider.Protos.WorkerAction result = GrpcClient.PostWorker(newWorker);

            Console.WriteLine($"New worker created with Id: {result.Worker.RowIdMessage.WorkerRowId}.");
        }

        #region Timer
        /// <summary>
        /// Set random interval to internal timer then starts it.
        /// </summary>
        public void Start()
        {
            Console.WriteLine(nameof(IntegrationService) + " just started.");
            Console.WriteLine("Press Enter to dispose service.\n");

            SetRandomInterval();
            _timer.Start();

        }

        /// <summary>
        /// Stops internal timer but allows to start it again.
        /// </summary>
        public void Stop() => _timer.Stop();

        /// <summary>
        /// Disposes internal timer
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
#if DEBUG
            Console.WriteLine("Timer disposed");
#endif
        }

        private void TimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            var random = new Random();
            bool create = random.Next(2) == 1;
            if (create)
            {
                CreateNewWorker();
            }
            else
            {
                UpdateRandomWorker().Wait();
            }

            SetRandomInterval();
        }

        private void SetRandomInterval()
        {
            _random = new Random();
            Console.WriteLine($"Next fire is: {NextFire}");

            _timer.Interval = _random.Next(ServiceOptions.MinimumInactiveTimePeriodMilliseconds, ServiceOptions.MaximumInactiveTimePeriodMilliseconds);
        }
        #endregion
    }
}
