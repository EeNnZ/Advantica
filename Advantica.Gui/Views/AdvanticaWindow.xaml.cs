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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AdvanticaWindow : Window
    {
        MainViewModel _viewModel;
        public AdvanticaWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            listBoxWorkers.ItemsSource = _viewModel.WorkersCollection;
            addWorkerTab.GotFocus += AddWorkerTab_GotFocus;
        }

        private void AddWorkerTab_GotFocus(object sender, RoutedEventArgs e)
        {
            listBoxWorkers.SelectedIndex = -1;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxWorkers.SelectedItem != null)
            {
                if (_viewModel.SelectedWorker != null)
                {
                    var updateWindow = new UpdateWindow((MainViewModel)DataContext);
                    updateWindow.Show();
                }
            }
            else MessageBox.Show("Select row before deleting", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
