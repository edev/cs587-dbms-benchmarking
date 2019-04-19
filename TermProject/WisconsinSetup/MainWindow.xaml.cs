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

        private SQC.SqlConnection Connection { get; set; } = new SQC.SqlConnection();

        private TableManager _tableManager = null;
        
        private Multipliers MultiplierCollection = new Multipliers();

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
            TbLog.TextChanged += LogTextChanged;
        }

        /* Scroll to the end of the log any time it changes. This event is fired when
         * the associated binding changes.
         */
        public void LogTextChanged(object sender, TextChangedEventArgs e)
        {
            TbLog.ScrollToEnd();
        }

        /* This function contains the exact instructions for making a given table,
         * expressed in terms of TableManager methods.
         TODO Make this async, or run in a background task, or whatever. Don't lock up the UI thread.
         */
        private void _makeTable(string tableName, long tableSize)
        {
            StringBuilder resultString = new StringBuilder();
            if (_tableManager == null)
            {
                _viewModel.LogEntry("Table manager is not ready.");
                return;
            }

            var watch = new Stopwatch();
            watch.Start();
            resultString.AppendLine($"Make table: {tableName}");
            resultString.AppendLine(_tableManager.DropTableIfExists(tableName));
            resultString.AppendLine(_tableManager.CreateTable(tableName));
            var relation = new Relation(tableName, tableSize);
            resultString.AppendLine($"Write CSV");
            relation.WriteCsv();
            // resultString.AppendLine(_tableManager.InsertRows(relation));
            resultString.AppendLine(_tableManager.BulkInsert(tableName, relation.CsvFilename));

            resultString.AppendLine("Done.");
            watch.Stop();
            // Copied from official example: https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?redirectedfrom=MSDN&view=netframework-4.8
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = watch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            resultString.AppendLine($"Elapsed time: {elapsedTime}");
            _viewModel.LogEntry(resultString.ToString());
        }

        private void _dropTable(string tableName)
        {
            if (_tableManager == null)
            {
                _viewModel.LogEntry("Table manager is not ready.");
                return;
            }
            _viewModel.LogEntry($"Drop table: {tableName}");
            _viewModel.LogEntry(_tableManager.DropTableIfExists(tableName));
        }

        private long Table1Size => Convert.ToInt64(TbRows1.Text) * (Multipliers.Mappings[(string)CbMultiplier1.SelectedValue]);
        private long Table2Size => Convert.ToInt64(TbRows3.Text) * (Multipliers.Mappings[(string)CbMultiplier2.SelectedValue]);
        private long Table3Size => Convert.ToInt64(TbRows3.Text) * (Multipliers.Mappings[(string)CbMultiplier3.SelectedValue]);

        private void BtnConnect_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _tableManager = null;
                if (Connection.State == ConnectionState.Open)
                {
                    _viewModel.LogEntry("Closing connection.");
                    Connection.Close();
                }

                // Bindings were NOT working properly, so let's just use the tried-and-true method
                // of pulling from and pushing to the form by hand. Simple, easy, and correct.
                Connection.ConnectionString = TbConnectionString.Text;
                Connection.Open();
                _tableManager = new TableManager(Connection);
                _viewModel.LogEntry("Connection open.");
            }
            catch (Exception exc)
            {
                _viewModel.LogEntry(exc.Message);
            }
            LblConnectionStatus.Content = Connection.State;
        }

        private void BtnDisconnect_OnClick(object sender, RoutedEventArgs e)
        {
            if (Connection.State == ConnectionState.Open)
            {
                _viewModel.LogEntry("Closing connection.");
            }
            Connection.Close();
            LblConnectionStatus.Content = Connection.State;
            _tableManager = null;
        }

        private void BtnCreateTable1_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _makeTable(TbTableName1.Text, Table1Size);
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
                _makeTable(TbTableName2.Text, Table2Size);
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
                _makeTable(TbTableName3.Text, Table3Size);
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
                _makeTable(TbTableName1.Text, Table1Size);
                _makeTable(TbTableName2.Text, Table2Size);
                _makeTable(TbTableName3.Text, Table3Size);
            }
            catch (System.FormatException)
            {
                _viewModel.LogEntry("# of rows must be a positive integer.");
            }
        }
         
        private void BtnDropTable1_OnClick(object sender, RoutedEventArgs e)
        {
            _dropTable(TbTableName1.Text);
        }

        private void BtnDropTable2_OnClick(object sender, RoutedEventArgs e)
        {
            _dropTable(TbTableName2.Text);
        }

        private void BtnDropTable3_OnClick(object sender, RoutedEventArgs e)
        {
            _dropTable(TbTableName3.Text);
        }

        private void BtnDropAll_OnClick(object sender, RoutedEventArgs e)
        {
            _dropTable(TbTableName1.Text);
            _dropTable(TbTableName2.Text);
            _dropTable(TbTableName3.Text);
        }
    }
}