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
        public SQC.SqlConnection Connection { get; set; } = new SQC.SqlConnection();
        // public SQC.SqlConnection Connection = new SQC.SqlConnection("Data Source=DYLAN-6CORE;Initial Catalog=wiscbench;Integrated Security=true;") { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnConnect_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                }

                // Bindings were NOT working properly, so let's just use the tried-and-true method
                // of pulling from and pushing to the form by hand. Simple, easy, and correct.
                Connection.ConnectionString = tbConnectionString.Text;
                Connection.Open();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            lblConnectionStatus.Content = Connection.State;
        }

        private void BtnDisconnect_OnClick(object sender, RoutedEventArgs e)
        {
            Connection.Close();
            lblConnectionStatus.Content = Connection.State;
        }
    }
}