using Advantica.GrpcServiceProvider;
using Advantica.GrpcServiceProvider.Protos;
using Advantica.Gui.Options;
using Advantica.Gui.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
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

            //_viewModel = new MainViewModel();
            //DataContext = _viewModel;
            //listBoxWorkers.ItemsSource = _viewModel.WorkersCollection;
            addWorkerTab.GotFocus += AddWorkerTab_GotFocus;

            //Grouping and filtering
            SetupGroupingAndFiltering();
        }

        private void SetupGroupingAndFiltering()
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listBoxWorkers.ItemsSource);
            PropertyGroupDescription propertyGroupDescription = new PropertyGroupDescription("FirstName[0]");
            view.GroupDescriptions.Add(propertyGroupDescription);
            view.Filter = (item) =>
            {
                if (string.IsNullOrEmpty(searchTextBox.Text)) return true;
                else
                    return ((WorkerMessage)item).FirstName.Contains(searchTextBox.Text, StringComparison.OrdinalIgnoreCase);
            };
        }

        private void searchTextBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(listBoxWorkers.ItemsSource).Refresh();
        }

        private void AddWorkerTab_GotFocus(object sender, RoutedEventArgs e)
        {
            listBoxWorkers.SelectedIndex = -1;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            updateWorkerTab.Focus();
        }
    }
}
