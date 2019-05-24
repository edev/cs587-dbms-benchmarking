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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SqlServerQueryRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private QueryRunnerViewModel _viewModel = new QueryRunnerViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        // ========
        // SECTION: Connect/disconnect
        // ========
        private void BtnConnect_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.Connect();
        }

        private void BtnDisconnect_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.Disconnect();
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ExecuteQuery();
        }
    }
}
