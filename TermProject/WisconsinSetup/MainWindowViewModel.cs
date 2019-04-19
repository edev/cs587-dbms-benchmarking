using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SQC = System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;

namespace WisconsinSetup
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public readonly SQC.SqlConnection Connection = new SQC.SqlConnection();

        private TableManager _tableManager = null;

        public MainWindowViewModel()
        {
            // Set the initially selected multipliers to the first one in the list.
            // We can't do this using initialization syntax, which is not shocking.
            Multiplier1 = Multipliers[0];
            Multiplier2 = Multipliers[0];
            Multiplier3 = Multipliers[0];
        }

        // ========
        // SECTION: INotifyPropertyChanged
        // ========

        public event PropertyChangedEventHandler PropertyChanged;

        /* When a property changes, we'll call NotifyPropertyChanged, which will infer
         * the name of the property that changed and fire an appropriate event.
         * We'll invoke this function any time we change a value, e.g. through
         * a property's setter. */
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (propertyName != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            Console.WriteLine($"{propertyName} changed.");
        }

        // ========
        // SECTION: Logging
        // ========

        private readonly StringBuilder _log = new StringBuilder();

        public string Log => _log.ToString();

        public string LogIndent { get; set; } = "    ";

        /* Add a new entry (line) to the log.
         * entry - the contents of the line (not including the newline)
         * nestingLevel - a numeric representation of the indentation level,
         *      expressed as the number of indents desired. This is NOT the
         *      number of spaces to indent! nestingLevel will be multiplied
         *      by LogIndent to obtain the final indentation text.
         */
        public void LogEntry(string entry, int nestingLevel = 0)
        {
            string indent = String.Concat(Enumerable.Repeat(LogIndent, nestingLevel));
            _log.AppendLine($"{indent}{entry}");
            Console.WriteLine($"Log:\n{Log}\n\n");
            NotifyPropertyChanged("Log");
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

        // Attempt to connect to the database using the connection string that the user
        // specified in the UI (which was hopefully bound via ConnectionString).
        public void Connect()
        {
            try
            {
                // For safety and DRY-ness, we'll close and clear any previous resources.
                // If we our connection attempt is successful, we'll open up again.
                ConnectionState = ConnectionState.Closed;
                _tableManager = null;
                if (Connection.State == ConnectionState.Open)
                {
                    LogEntry("Closing connection.");
                    Connection.Close();
                }

                Connection.Open();

                // If we reach this point without raising an error, we have an open connection,
                // and all is well. Time to open up shop!
                _tableManager = new TableManager(Connection);
                ConnectionState = Connection.State;
                LogEntry("Connection open.");
            }
            catch (Exception exc)
            {
                // Connection.Open() threw an error, most likely.
                // Log it and move on.
                LogEntry(exc.Message);
            }
        }

        public void Disconnect()
        {
            if (Connection.State == ConnectionState.Open)
            {
                LogEntry("Closing connection.");
            }

            Connection.Close();
            ConnectionState = Connection.State;
            _tableManager = null;
        }

        // ========
        // SECTION: Table Sizes
        // ========

        

        // ========
        // SECTION: Table Size Multipliers
        // ========

        // Our collection of multipliers, which populates the selectable list in the UI.
        public ObservableCollection<Multiplier> Multipliers = new ObservableCollection<Multiplier>
        {
            new Multiplier("1x", 1),
            new Multiplier("Thousand", 1000),
            new Multiplier("Million", 1000000)
        };

        // NOTE: Multipliers for the three tables are initialized in the constructor.

        // The currently selected multiplier for table 1.
        private Multiplier _multiplier1;
        public Multiplier Multiplier1
        {
            get => _multiplier1;
            set
            {
                _multiplier1 = value;
                NotifyPropertyChanged();
            }
        }

        // The currently selected multiplier for table 2.
        private Multiplier _multiplier2;
        public Multiplier Multiplier2
        {
            get => _multiplier2;
            set
            {
                _multiplier2 = value;
                NotifyPropertyChanged();
            }
        }

        // The currently selected multiplier for table 3.
        private Multiplier _multiplier3;
        public Multiplier Multiplier3
        {
            get => _multiplier3;
            set
            {
                _multiplier3 = value;
                NotifyPropertyChanged();
            }
        }

        // ========
        // SECTION: Drop Table
        // ========

        public void DropTable(string tableName)
        {
            if (_tableManager == null)
            {
                LogEntry("Table manager is not ready.");
                return;
            }
            LogEntry($"Drop table: {tableName}");
            LogEntry(_tableManager.DropTableIfExists(tableName), 1); // TODO Clean up the indentation mess elsewhere!
        }

        // ========
        // SECTION: Make Table
        // ========

        /* This function contains the exact instructions for making a given table,
         * expressed in terms of TableManager methods.
         TODO Make this async, or run in a background task, or whatever. Don't lock up the UI thread.
         */
        public void MakeTable(string tableName, long tableSize)
        {
            if (_tableManager == null)
            {
                LogEntry("Table manager is not ready.");
                return;
            }

            // We'll time the whole operation so we can report elapsed time at the end.
            var watch = new Stopwatch();
            watch.Start();

            // First, we'll drop and re-create the table.
            LogEntry($"Make table: {tableName}");
            LogEntry(_tableManager.DropTableIfExists(tableName), 1);
            LogEntry(_tableManager.CreateTable(tableName), 1);

            // Next, we'll generate our rows and write our CSV file.
            var relation = new Relation(tableName, tableSize);
            LogEntry($"Write CSV"); // TODO Save some time by incorporating a row count into CSV filename and reusing it if it exists!
            relation.WriteCsv();

            // Finally, we'll issue a bulk load command to our DBMS.
            LogEntry(_tableManager.BulkInsert(tableName, relation.CsvFilename));

            // Done!
            LogEntry("Done.");
            watch.Stop();

            // Copied from official Microsoft example:
            // https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?redirectedfrom=MSDN&view=netframework-4.8
            TimeSpan ts = watch.Elapsed;  // Get the elapsed time as a TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            LogEntry($"Elapsed time: {elapsedTime}");
        }
    }
}
