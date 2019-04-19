using System;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using SQC = System.Data.SqlClient;
using System.Linq;
using System.Net.Configuration;
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

namespace WisconsinSetup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        private Multipliers MultiplierCollection = new Multipliers();

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
            TbLog.TextChanged += LogTextChanged; // Required so we can scroll to end when log entries come in.
        }

        /* Scroll to the end of the log any time it changes. This event is fired when
         * the associated binding changes.
         */
        public void LogTextChanged(object sender, TextChangedEventArgs e)
        {
            TbLog.ScrollToEnd();
        }

        private long Table1Size => Convert.ToInt64(TbRows1.Text) * (Multipliers.Mappings[(string)CbMultiplier1.SelectedValue]);
        private long Table2Size => Convert.ToInt64(TbRows3.Text) * (Multipliers.Mappings[(string)CbMultiplier2.SelectedValue]);
        private long Table3Size => Convert.ToInt64(TbRows3.Text) * (Multipliers.Mappings[(string)CbMultiplier3.SelectedValue]);

        /* Ask the ViewModel to connect using the user's connection string. */
        private void BtnConnect_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.Connect();
        }

        private void BtnDisconnect_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.Disconnect();
        }

        private void BtnCreateTable1_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.MakeTable(TbTableName1.Text, Table1Size);
            }
            catch (System.FormatException)
            {
                _viewModel.LogEntry("# of rows must be a positive integer.");
            }
        }

        private void BtnCreateTable2_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.MakeTable(TbTableName2.Text, Table2Size);
            }
            catch (System.FormatException)
            {
                _viewModel.LogEntry("# of rows must be a positive integer.");
            }
        }

        private void BtnCreateTable3_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.MakeTable(TbTableName3.Text, Table3Size);
            }
            catch (System.FormatException)
            {
                _viewModel.LogEntry("# of rows must be a positive integer.");
            }
        }

        private void BtnCreateAll_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.MakeTable(TbTableName1.Text, Table1Size);
                _viewModel.MakeTable(TbTableName2.Text, Table2Size);
                _viewModel.MakeTable(TbTableName3.Text, Table3Size);
            }
            catch (System.FormatException)
            {
                _viewModel.LogEntry("# of rows must be a positive integer.");
            }
        }
         
        private void BtnDropTable1_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.DropTable(TbTableName1.Text);
        }

        private void BtnDropTable2_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.DropTable(TbTableName2.Text);
        }

        private void BtnDropTable3_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.DropTable(TbTableName3.Text);
        }

        private void BtnDropAll_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.DropTable(TbTableName1.Text);
            _viewModel.DropTable(TbTableName2.Text);
            _viewModel.DropTable(TbTableName3.Text);
        }
    }
}