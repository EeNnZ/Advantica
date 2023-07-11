using Advantica.GrpcServiceProvider.Protos;
using Advantica.Gui.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Advantica.Gui.Views
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        MainViewModel _viewModel;
        public UpdateWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            textBoxWorkerFirstName.Text = _viewModel.SelectedWorker?.FirstName;
            textBoxWorkerLastName.Text = _viewModel.SelectedWorker?.LastName;
            textBoxWorkerMiddleName.Text = _viewModel.SelectedWorker?.MiddleName;
            textBoxWorkerBirtday.Text = _viewModel.SelectedWorker?.Birthday.ToString();
            textBoxWorkerHasChildren.Text = _viewModel.SelectedWorker?.HasChildren.ToString();
            textBoxWorkerSex.Text = _viewModel.SelectedWorker?.Sex.ToString();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var workerMessage = new WorkerMessage()
            {
                FirstName = textBoxWorkerFirstName.Text,
                LastName = textBoxWorkerLastName.Text,
                MiddleName = textBoxWorkerMiddleName.Text,
                Birthday = long.TryParse(textBoxWorkerBirtday.Text, out long binDate) ? binDate : 0,
                HasChildren = bool.TryParse(textBoxWorkerHasChildren.Text, out bool hasChildren) && hasChildren,
                Sex = string.Compare(textBoxWorkerSex.Text, Sex.Male.ToString(), StringComparison.OrdinalIgnoreCase) == 0 ? Sex.Female : Sex.Male
            };
            _viewModel.UpdateWorkerCommand.Execute(workerMessage);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
