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
    /// <summary>
    ///     The View Model for the WisconsinSetup.MainWindow view. This class acts as the binding source
    ///     for most properties of said view and provides the interface for communicating with underlying models
    ///     like WisconsinSetup.TableManager.
    /// </summary>
    class MainWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        ///     The database connection.
        /// </summary>
        public readonly SQC.SqlConnection Connection = new SQC.SqlConnection();

        // The TableManager instance used for SQL operations.
        private TableManager _tableManager = null;

        /// <summary>
        ///     Creates a new MainWindowViewModel using default options.
        /// </summary>
        public MainWindowViewModel()
        {
            // Set the initially selected multipliers as sensible defaults.
            // We can't do this using initialization syntax, which is not shocking.
            Multiplier1 = Multipliers[1];
            Multiplier2 = Multipliers[2];
            Multiplier3 = Multipliers[2];
            ConnectionString = "Data Source=DYLAN-6CORE;Initial Catalog=wiscbench;Integrated Security=true;";
        }

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
        // SECTION: Logging
        // ========

        // The backing datatype that stores a mutable copy of the log.
        private readonly StringBuilder _log = new StringBuilder();

        /// <summary>
        ///     Read-only property that represents the current contents of the log. Any UI elements that need
        ///     to show the log should bind to this property.
        /// </summary>
        public string Log
        {
            get
            {
                lock (_log)
                {
                    return _log.ToString();
                }
            }
        }                                                                                                              

        public string LogIndent { get; set; } = "    ";

        /// <summary>
        ///     Add a new entry (line) to the log.
        /// </summary>
        /// <param name="entry">
        ///     The contents of the line (not including the newline)
        /// </param>
        /// <param name="nestingLevel">
        ///     A numeric representation of the indentation level, expressed as the number of indents desired. 
        ///     This is NOT the number of spaces to indent! nestingLevel will be multiplied by LogIndent to obtain 
        ///     the final indentation text.
        /// </param>
        public void LogEntry(string entry, int nestingLevel = 0)
        {
            lock (_log)
            {
                string indent = String.Concat(Enumerable.Repeat(LogIndent, nestingLevel));
                _log.AppendLine($"{indent}{entry}");
                NotifyPropertyChanged("Log");
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
                _tableManager = null;
                if (Connection.State == ConnectionState.Open)
                {
                    LogEntry("Closing connection.");
                    Connection.Close();
                }

                Connection.Open();

                // If we reach this point without raising an error, we have an open connection, and all is well.
                // Time to open up shop!
                _tableManager = new TableManager(Connection);
                ConnectionState = Connection.State;
                LogEntry("Connection open.");
            }
            catch (Exception exc)
            {
                // Connection.Open() threw an error, most likely. Log it and move on.
                LogEntry(exc.Message);
            }
        }

        /// <summary>
        ///     Close the connection to the database.
        /// </summary>
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

        // TODO See whether to use a converter and validation rule here. Seems like a lot of work and isn't very well explained on docs.microsoft.com.
        // For now, we do NOT bind these, because it saves quite a bit of work to simply attempt to convert them
        // on-demand in the MainWindow code-behind. It's not ideal, but it's a lot less work,
        // and it's plenty good for this project.

        // ========
        // SECTION: Table Size Multipliers
        // ========

        /// <summary>
        ///     Our collection of multipliers, which populates the selectable list in the UI.
        /// </summary>
        public ObservableCollection<Multiplier> Multipliers { get; } = new ObservableCollection<Multiplier>
        {
            new Multiplier("1x", 1),
            new Multiplier("Thousand", 1000),
            new Multiplier("Million", 1000000)
        };

        // NOTE: Multipliers for the three tables are initialized in the constructor.

        // The currently selected multiplier for table 1.
        private Multiplier _multiplier1;

        /// <summary>
        ///     Returns the currently-selected multiplier for the first table.
        /// </summary>
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

        /// <summary>
        ///     Returns the currently-selected multiplier for the second table.
        /// </summary>
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

        /// <summary>
        ///     Returns the currently-selected multiplier for the third table.
        /// </summary>
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

        /// <summary>
        ///     Asks the TableManager to drop the given table, if it exists.
        /// </summary>
        /// <param name="tableName"></param>
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

        // All MakeTable instances will lock around this object in order to serialize their access.
        private object makeTableLock = new object();

        /// <summary>
        ///     This method drops the named table if it exists, creates a new table, and populates it with
        ///     the specified number of entries.
        /// </summary>
        /// <param name="tableName">The SQL name of the table to (re)create.</param>
        /// <param name="tableSize">The number of records to generate and load into the new table.</param>
        public void MakeTable(string tableName, long tableSize)
        {
            lock (makeTableLock)
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
                try
                {
                    relation.WriteCsv();
                }
                catch (Exception exc)
                {
                    LogEntry(exc.Message);
                }

                // Finally, we'll issue a bulk load command to our DBMS.
                LogEntry(_tableManager.BulkInsert(tableName, relation.CsvFilename));

                // Done!
                LogEntry("Done.");
                watch.Stop();

                // Copied from official Microsoft example:
                // https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?redirectedfrom=MSDN&view=netframework-4.8
                TimeSpan ts = watch.Elapsed; // Get the elapsed time as a TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                LogEntry($"Elapsed time: {elapsedTime}");
            }
        }

        // ========
        // SECTION: Delete all CSVs
        // ========

        /// <summary>
        ///     Deletes all .csv files in the current directory.
        /// </summary>
        public void DeleteAllCsvs()
        {
            // Adapted from solution 1 of:
            // https://www.codeproject.com/questions/74057/how-to-delete-all-files-with-specific-extension
            foreach (string filename in System.IO.Directory.GetFiles(".", "*.csv"))
            {
                LogEntry($"Deleting {filename}");
                System.IO.File.Delete(filename);
            }
        }
    }
}
