using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WisconsinSetup
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
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

    }
}
