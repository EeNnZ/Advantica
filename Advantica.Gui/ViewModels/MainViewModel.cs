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

namespace Advantica.Gui.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly WorkerIntegration.WorkerIntegrationClient _grpcClient;


        [ObservableProperty]
        private string? _status;

        
        private WorkerMessage? _selectedWorker;
        public WorkerMessage? SelectedWorker
        {
            get 
            {
                if (_selectedWorker == null)
                {
                    _selectedWorker = new WorkerMessage();
                }
                return _selectedWorker;
            }
            set
            {
                SetProperty(ref _selectedWorker, value);
            }
        }

        public ObservableCollection<WorkerMessage>? WorkersCollection { get; set; }

        public IOptions Options { get; }

        public MainViewModel()
        {
            var opProvider = new OptionsProvider();
            Options = opProvider.GetOptions();

            WorkersCollection = new ObservableCollection<WorkerMessage>();
            _grpcClient = new GrpcClientProvider(Options.Url).GetWorkerIntegrationClient();
        }

        [RelayCommand]
        private async Task GetWorkersAsync()
        {
            WorkersCollection?.Clear();
            var responseStream = _grpcClient.GetWorkerStream(new EmptyMessage()).ResponseStream;

            while(await responseStream.MoveNext(new System.Threading.CancellationToken()))
            {
                var response = responseStream.Current;
                WorkersCollection?.Add(response.Worker);
            }
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
                await GetWorkersAsync();
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
                await GetWorkersAsync();
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
                await GetWorkersAsync();
            }
        }

    }
}
