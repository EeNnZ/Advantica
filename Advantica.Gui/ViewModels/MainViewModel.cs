using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Advantica.GrpcServiceProvider.Protos;
using CommunityToolkit.Mvvm.Collections;
using Advantica.GrpcServiceProvider;
using System.Net.WebSockets;
using System.ComponentModel.DataAnnotations;
using Advantica.Gui.Options;
using System.Windows.Threading;
using Grpc.Core;
using System.Linq;

namespace Advantica.Gui.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly WorkerIntegration.WorkerIntegrationClient _grpcClient;
        private System.Timers.Timer _timer;
        private readonly Dispatcher _dispatcher;

        [ObservableProperty]
        private string? _status = "Ready";

        [ObservableProperty]
        private bool _dynamicCheck = false;

        private WorkerMessage? _selectedWorker;
        //TODO: Bug: after adding or updating selected worker freezes and not changes until GetWorkers being called manually
        public WorkerMessage? SelectedWorker
        {
            get
            {
                _selectedWorker ??= new WorkerMessage();
                return _selectedWorker;
            }
            set
            {
                SetProperty(ref _selectedWorker, value);
            }
        }

        public ObservableCollection<WorkerMessage>? WorkersCollection { get; set; }

        public IOptions Options { get; }

        //public MainViewModel()
        //{
        //    _dispatcher = Dispatcher.CurrentDispatcher;

        //    var opProvider = new OptionsProvider();
        //    Options = opProvider.GetOptions();

        //    WorkersCollection = new ObservableCollection<WorkerMessage>();
        //    _grpcClient = new GrpcClientProvider(Options.Url).GetWorkerIntegrationClient();

        //    _timer = new System.Timers.Timer(30_000);
        //    _timer.Elapsed += Timer_Elapsed;
        //    _timer.Start();
        //}

        public MainViewModel(IOptionsProvider optionsProvider)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;

            Options = optionsProvider.GetOptions();
            _grpcClient = new GrpcClientProvider(Options.Url).GetWorkerIntegrationClient();


            WorkersCollection = new ObservableCollection<WorkerMessage>();

            _timer = new System.Timers.Timer(30_000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }



        private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (!DynamicCheck) return;

            DatabaseModifiedMessage response;
            try
            {
                response = await CheckIfDatabaseHasModified();
            }
            catch (NullReferenceException ex)
            {
                Status = ex.Message;
                return;
            }
            
            if (response.IsModified)
            {
                await _dispatcher.InvokeAsync(GetWorkersAsync);
            }
            else
            {
                Status = "All synced";
            }
        }

        private async Task<DatabaseModifiedMessage> CheckIfDatabaseHasModified()
        {
            Status = "Checking...";
            if (WorkersCollection == null)
            {
                return await Task.FromException<DatabaseModifiedMessage>(new NullReferenceException(nameof(WorkersCollection)));
            }

            var call = _grpcClient.CheckIfDatabaseModified();
            var workersLocal = WorkersCollection.ToArray();
            foreach (var worker in workersLocal)
            {
                await call.RequestStream.WriteAsync(worker);
            }

            await call.RequestStream.CompleteAsync();

            var serverResponse = call.ResponseAsync;

            return await serverResponse;
        }

        [RelayCommand]
        private async Task GetWorkersAsync()
        {
            this.Status = "Updating...";

            WorkersCollection?.Clear();
            var responseStream = _grpcClient.GetWorkerStream(new EmptyMessage()).ResponseStream;

            while(await responseStream.MoveNext(new System.Threading.CancellationToken()))
            {
                var response = responseStream.Current;
                WorkersCollection?.Add(response.Worker);
            }

            this.Status = "Ready";
        }

        [RelayCommand]
        private async Task CreateNewWorkerAsync(WorkerMessage workerMessage)
        {
            if (workerMessage != null)
            {
                workerMessage.RowIdMessage = new WorkerRowIdMessage() { WorkerRowId = 0 };
                var createAction = new WorkerAction()
                {
                    ActionType = GrpcServiceProvider.Protos.Action.Create,
                    Worker = workerMessage
                };
                var result = await _grpcClient.PostWorkerAsync(createAction);
                SelectedWorker = null;
                await GetWorkersCommand.ExecuteAsync(null);
            }
            
        }

        [RelayCommand]
        private async Task UpdateWorkerAsync(WorkerMessage workerMessage)
        {
            if (workerMessage != null)
            {
                workerMessage.RowIdMessage = SelectedWorker?.RowIdMessage;
                var updateAction = new WorkerAction()
                {
                    ActionType = GrpcServiceProvider.Protos.Action.Update,
                    Worker = workerMessage
                };
                var result = await _grpcClient.PutWorkerAsync(updateAction);
                SelectedWorker = null;
                await GetWorkersCommand.ExecuteAsync(null);
            }
        }

        [RelayCommand]
        private async Task DeleteWorkerAsync(WorkerMessage workerMessage)
        {
            if (SelectedWorker != null)
            {
                var result = await _grpcClient.DeleteWorkerAsync(SelectedWorker.RowIdMessage);
                SelectedWorker = null;
                //TODO: Use result?
                await GetWorkersCommand.ExecuteAsync(null);
            }
        }

    }
}
