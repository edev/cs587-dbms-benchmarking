using System;
using System.Data;
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
        private SQC.SqlConnection Connection { get; set; } = new SQC.SqlConnection();

        private TableManager _tableManager = null;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void _logLine(string message)
        {
            if (message == null)
            {
                return;
            }

            Console.WriteLine(message);
        }

        /* This function contains the exact instructions for making a given table,
         * expressed in terms of TableManager methods.
         */
        private void _makeTable(string tableName)
        {
            _logLine(_tableManager?.DropTableIfExists(tableName));
            _logLine(_tableManager?.CreateTable(tableName));
        }

        private void BtnConnect_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _tableManager = null;
                if (Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                }

                // Bindings were NOT working properly, so let's just use the tried-and-true method
                // of pulling from and pushing to the form by hand. Simple, easy, and correct.
                Connection.ConnectionString = TbConnectionString.Text;
                Connection.Open();
                _tableManager = new TableManager(Connection);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            LblConnectionStatus.Content = Connection.State;
        }

        private void BtnDisconnect_OnClick(object sender, RoutedEventArgs e)
        {
            Connection.Close();
            LblConnectionStatus.Content = Connection.State;
            _tableManager = null;
        }

        private void BtnCreateTable1_OnClick(object sender, RoutedEventArgs e)
        {
            _makeTable(TbTableName1.Text);
        }

        private void BtnCreateTable2_OnClick(object sender, RoutedEventArgs e)
        {
            _makeTable(TbTableName2.Text);
        }

        private void BtnCreateTable3_OnClick(object sender, RoutedEventArgs e)
        {
            _makeTable(TbTableName3.Text);
        }

        private void BtnCreateAll_OnClick(object sender, RoutedEventArgs e)
        {
            _makeTable(TbTableName1.Text);
            _makeTable(TbTableName2.Text);
            _makeTable(TbTableName3.Text);
        }

        private void BtnDropTable1_OnClick(object sender, RoutedEventArgs e)
        {
            _logLine(_tableManager?.DropTableIfExists(TbTableName1.Text));
        }

        private void BtnDropTable2_OnClick(object sender, RoutedEventArgs e)
        {
            _logLine(_tableManager?.DropTableIfExists(TbTableName2.Text));
        }

        private void BtnDropTable3_OnClick(object sender, RoutedEventArgs e)
        {
            _logLine(_tableManager?.DropTableIfExists(TbTableName3.Text));
        }

        private void BtnDropAll_OnClick(object sender, RoutedEventArgs e)
        {
            _logLine(_tableManager?.DropTableIfExists(TbTableName1.Text));
            _logLine(_tableManager?.DropTableIfExists(TbTableName2.Text));
            _logLine(_tableManager?.DropTableIfExists(TbTableName3.Text));
        }
    }
}