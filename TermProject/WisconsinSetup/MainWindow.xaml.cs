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

        // ========
        // SECTION: Create table(s)
        // ========

        private long? _tryConvertToLong(string value)
        {
            try
            {
                long v = Convert.ToInt64(value);
                if (v < 0)
                {
                    // Conversion succeeded, but the number is negative and thus invalid.
                    return null;
                }

                // Valid conversion!
                return v;
            }
            catch
            {
                // Conversion from string to long failed.
                return null;
            }
        }

        private void tryMakeTable(string tableName, string numRowsString, long multiplier)
        {
            long? numRows = _tryConvertToLong(numRowsString);
            if (!numRows.HasValue)
            {
                // Conversion failed.
                _viewModel.LogEntry($"{numRowsString} is not a valid number of rows.");
                return;
            }

            _viewModel.MakeTable(tableName, numRows.Value * multiplier);
        }

        private void BtnCreateTable1_OnClick(object sender, RoutedEventArgs e)
        {
            tryMakeTable(TbTableName1.Text, TbRows1.Text, _viewModel.Multiplier1.ToInt64());
        }

        private void BtnCreateTable2_OnClick(object sender, RoutedEventArgs e)
        {
            tryMakeTable(TbTableName2.Text, TbRows2.Text, _viewModel.Multiplier2.ToInt64());
        }

        private void BtnCreateTable3_OnClick(object sender, RoutedEventArgs e)
        {
            tryMakeTable(TbTableName3.Text, TbRows3.Text, _viewModel.Multiplier3.ToInt64());
        }

        private void BtnCreateAll_OnClick(object sender, RoutedEventArgs e)
        {
            tryMakeTable(TbTableName1.Text, TbRows1.Text, _viewModel.Multiplier1.ToInt64());
            tryMakeTable(TbTableName2.Text, TbRows2.Text, _viewModel.Multiplier2.ToInt64());
            tryMakeTable(TbTableName2.Text, TbRows3.Text, _viewModel.Multiplier3.ToInt64());
        }
         
        // ========
        // SECTION: Drop table(s)
        // ========

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