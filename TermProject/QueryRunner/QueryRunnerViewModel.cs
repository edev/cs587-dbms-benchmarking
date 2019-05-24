using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;

namespace SqlServerQueryRunner
{
    class QueryRunnerViewModel : INotifyPropertyChanged
    {
        public QueryRunnerViewModel()
        {
            ConnectionString = "Data Source=DYLAN-6CORE;Initial Catalog=wiscbench;Integrated Security=true;";
        }

        /// <summary>
        ///     The database connection.
        /// </summary>
        public readonly SqlConnection Connection = new SqlConnection();

        // ========
        // SECTION: INotifyPropertyChanged
        // ========

        public event PropertyChangedEventHandler PropertyChanged;

        // When a property changes, we'll call NotifyPropertyChanged, which will infer the name of the property
        // that changed and fire an appropriate event. We'll invoke this function any time we change a value,
        // e.g. through a property's setter. (In rare cases when we need to notify for a different property,
        // we can do that by overriding propertyName.)
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (propertyName != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // ========
        // SECTION: Connect/disconnect
        // ========

        // Connection state, with notifier.
        private ConnectionState _connectionState = ConnectionState.Closed;
        public ConnectionState ConnectionState
        {
            get => _connectionState;
            set
            {
                _connectionState = value;
                NotifyPropertyChanged();
            }
        }

        // Connection string, with notifier.
        public string ConnectionString
        {
            get => Connection.ConnectionString;
            set
            {
                Connection.ConnectionString = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        ///     Attempt to connect to the database using the connection string that the user specified in the UI
        ///     (which was hopefully bound via ConnectionString).
        /// </summary>
        public void Connect()
        {
            try
            {
                // For safety and DRY-ness, we'll close and clear any previous resources.
                // If we our connection attempt is successful, we'll open up again.
                ConnectionState = ConnectionState.Closed;
                if (Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                }

                Connection.Open();

                // If we reach this point without raising an error, we have an open connection, and all is well.
                // Time to open up shop!
                ConnectionState = Connection.State;
            }
            catch (Exception exc)
            {
            }
        }

        /// <summary>
        ///     Close the connection to the database.
        /// </summary>
        public void Disconnect()
        {
            if (Connection.State == ConnectionState.Open)
            {
            }

            Connection.Close();
            ConnectionState = Connection.State;
        }

        // ========
        // SECTION: Query Processing
        // ========

        private string _sql = "Enter SQL here";
        public string SQL
        {
            get => _sql;
            set
            {
                _sql = value;
                NotifyPropertyChanged();
            }
        }

        private string _queryStatus;
        public string QueryStatus
        {
            get => _queryStatus;
            set
            {
                _queryStatus = value;
                NotifyPropertyChanged();
            }
        }

        private string _elapsedTime;
        public string ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                _elapsedTime = $"Elapsed time: {value} ms";
                NotifyPropertyChanged();
            }
        }

        public void ExecuteQuery()
        {
            if (ConnectionState != ConnectionState.Open)
            {
                // Fail, but notify the user unobtrusively.
                QueryStatus = "No connection";
                ElapsedTime = "0";
                return;
            }

            // Prepare everything BEFORE starting the timer.
            var sqlCommand = new SqlCommand(SQL, Connection);
            QueryStatus = "Running";

            // Start the background process that will take care of the rest.
            _timeQuery(sqlCommand);
        }

        private async Task _timeQuery(SqlCommand sqlCommand)
        {
            // Define our query-running logic in a local function so we can run it asynchronously
            // and not lock up the UI thread.
            string timeQuery()
            {
                Stopwatch watch = Stopwatch.StartNew();
                try
                {
                    sqlCommand.ExecuteNonQuery();
                    watch.Stop();

                    // Success!
                    QueryStatus = "Done";
                }
                catch (Exception e)
                {
                    watch.Stop();
                    QueryStatus = "Error";
                }
                return watch.ElapsedMilliseconds.ToString();
            }

            ElapsedTime = await Task.Run(() => timeQuery());
        }

    }
}
