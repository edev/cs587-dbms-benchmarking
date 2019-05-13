using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeometryGenerator
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        internal MainWindowViewModel()
        {
            Generate = new GenerateCommandHandler(() =>
            {
                // Guard against empty filenames.
                if (Filename == null || Filename.Trim() == "")
                {
                    return;
                }

                Task.Run(() => _writePointsToFile(Filename, GridMin, GridMax, GridMin, GridMax, GridInterval, RandomJitter, Precision));
                
            });
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

            // Recompute NumberOfPoints every time a property changes.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumberOfPoints)));
        }

        // ========
        // SECTION: Grid
        // ========

        private double _gridMin = 0;
        public double GridMin
        {
            get => _gridMin;
            set
            {
                _gridMin = value;
                NotifyPropertyChanged();
            }
        }

        private double _gridMax = 999;
        public double GridMax
        {
            get => _gridMax;
            set
            {
                _gridMax = value;
                NotifyPropertyChanged();
            }
        }

        private double _gridinterval = 1;
        public double GridInterval
        {
            get => _gridinterval;
            set
            {
                _gridinterval = value;
                NotifyPropertyChanged();
            }
        }

        public double NumberOfPoints => _calculateNumberOfPoints();

        /// <summary>
        /// Recomputes the number of points that the parameters provided will generate.
        /// </summary>
        private double _calculateNumberOfPoints()
        {
            double pointsInARow = Math.Floor((GridMax - GridMin) / GridInterval) + 1;
            return Math.Pow(pointsInARow, 2);
        }

        // ========
        // SECTION: Random number generation
        // ========

        private double _randomJitter = 2;
        public double RandomJitter
        {
            get => _randomJitter;
            set
            {
                _randomJitter = value;
                NotifyPropertyChanged();
            }
        }

        // ========
        // SECTION: Output specification
        // ========

        private uint _precision = 4;
        public uint Precision
        {
            get => _precision;
            set
            {
                _precision = value;
                NotifyPropertyChanged();
            }
        }

        private string _filename = "randomPoints.csv";
        public string Filename
        {
            get => _filename;
            set
            {
                _filename = value;
                NotifyPropertyChanged();
            }
        }

        // ========
        // SECTION: Generate output
        // ========
        public GenerateCommandHandler Generate { get; } 
        public class GenerateCommandHandler : ICommand
        {
            private readonly Action _action;

            public event EventHandler CanExecuteChanged;

            public GenerateCommandHandler(Action action)
            {
                _action = action;
            }

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                _action();
            }
        }

        private void _writePointsToFile(string filename, double xmin, double xmax, double ymin, double ymax, double interval, double jitter, uint precision)
        {
            StreamWriter file;
            try
            {
                file = File.CreateText(filename);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }

            // File is open. 

            // Generate the correct double format string to use for file output.
            string doubleFormat;
            if (precision == 0)
            {
                doubleFormat = "0";
            }
            else
            {
                doubleFormat = "0." + String.Concat(Enumerable.Repeat("0", (int)precision));
            }
            // Interpolate the double format string into a point format string that we can use later.
            string pointFormat = $"{{0:{doubleFormat}}},{{1:{doubleFormat}}}\n";

            // Write each point to the file as it comes out of the generator.
            foreach (Point p in new RandomPointGenerator(xmin, xmax, ymin, ymax, interval, jitter))
            {
                file.Write(String.Format(pointFormat, p.X, p.Y));
            }

            file.Close();
            MessageBox.Show("File written.");
        }
    }
}
