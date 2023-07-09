using Advantica.IntegrationSystem.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Timer = System.Timers.Timer;

namespace Advantica.IntegrationSystem.Services
{
    internal class IntegrationService : IDisposable
    {
        //TODO: move to appsettings
        private const int ONE_MINUTE = 60_000;
        private const int TWENTY_MINUTES = 1_200_000;

        private Random _random;
        private Timer _timer;

        private string[] _firstNames;
        private string[] _lastNames;


        public Timer Timer => _timer;
        public double NextFire
        {
            get
            {
#if !DEBUG
                return _timer.Interval / ONE_MINUTE;
#else
                return _timer.Interval / 1000;
#endif
            }
        }

        public IntegrationService()
        {
            _firstNames = File.ReadAllLines("Files\\first_names.txt");
            _lastNames = File.ReadAllLines("Files\\last_names.txt");

            _random = new Random();
            _timer = new Timer();

            _timer.Elapsed += TimerElapsed;
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

        public async Task UpdateRandomWorker()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5249");
            var client = new WorkerIntegration.WorkerIntegrationClient(channel);

            var data = client.GetWorkerStream(new EmptyMessage());

            var responseStream = data.ResponseStream;

            var workers = new List<WorkerMessage>();
            while (await responseStream.MoveNext(new CancellationToken()))
            {
                var response = responseStream.Current;
                workers.Add(response.Worker);
            }

            if (workers.Count < 15)
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

            var workerActionPut = new WorkerAction()
            {
                ActionType = Protos.Action.Update,
                Worker = randomWorker
            };

            WorkerAction result = client.PutWorker(workerActionPut);

            Console.WriteLine($"Worker: {randomWorker.RowIdMessage.WorkerRowId} updated. New last name: {randomWorker.LastName}, HasChildren: {randomWorker.HasChildren}");
        }

        public void CreateNewWorker()
        {
            var random = new Random();

            var newWorker = new WorkerAction()
            {
                ActionType = Advantica.IntegrationSystem.Protos.Action.Create,
                Worker = new WorkerMessage()
                {
                    FirstName = _firstNames[random.Next(_firstNames.Length - 1)],
                    MiddleName = _firstNames[random.Next(_firstNames.Length - 1)],
                    LastName = _lastNames[random.Next(_lastNames.Length - 1)],
                    Birthday = new DateTime(random.Next(1950, 2004), random.Next(1, 12), random.Next(1, 30)).ToBinary(),
                    HasChildren = random.Next(2) == 1,
                    //TODO: Gender api
                    Sex = (Sex)random.Next(1,3),
                    RowIdMessage = new WorkerRowIdMessage() { WorkerRowId = 0 }
                }
            };

            //TODO: add appsettings
            using var channel = GrpcChannel.ForAddress("http://localhost:5249");
            var client = new WorkerIntegration.WorkerIntegrationClient(channel);

            WorkerAction result = client.PostWorker(newWorker);

            Console.WriteLine($"New worker created with Id: {result.Worker.RowIdMessage.WorkerRowId}.");
        }

        private void SetRandomInterval()
        {
            _random = new Random();
#if DEBUG
            _timer.Interval = _random.Next(5000, 10000);
            Console.WriteLine($"Next fire is: {NextFire}");
#else
            _timer.Interval = _random.Next(ONE_MINUTE, TWENTY_MINUTES);
#endif
        }
        #endregion
    }
}
