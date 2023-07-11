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

            WorkerMessage? worker = _viewModel.SelectedWorker;
            if (worker != null)
            {
                textBoxWorkerFirstName.Text = worker.FirstName;
                textBoxWorkerLastName.Text = worker.LastName;
                textBoxWorkerMiddleName.Text = worker.MiddleName;
                datePickerWorkerBirtday.Text = DateTime.FromBinary(worker.Birthday).ToString();
                comboBoxWorkerHasChildren.Text = worker.HasChildren.ToString();
                textBoxWorkerSex.Text = worker.Sex.ToString();
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Sex sex;
            if (textBoxWorkerSex.Text.ToLower() == Sex.Male.ToString().ToLower())
            {
                sex = Sex.Male;
            }
            else if (textBoxWorkerSex.Text.ToLower() == Sex.Female.ToString().ToLower())
            {
                sex = Sex.Female;
            }
            else
            {
                sex = Sex.UnknownSex;
            }

            var workerMessage = new WorkerMessage()
            {
                FirstName = textBoxWorkerFirstName.Text,
                LastName = textBoxWorkerLastName.Text,
                MiddleName = textBoxWorkerMiddleName.Text,
                Birthday = datePickerWorkerBirtday.DisplayDate.ToBinary(),
                HasChildren = bool.TryParse(comboBoxWorkerHasChildren.Text, out bool hasChildren) && hasChildren,
                Sex = sex
            };
            _viewModel.UpdateWorkerCommand.Execute(workerMessage);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
