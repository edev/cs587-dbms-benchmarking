using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using EnsureThat;

namespace WisconsinSetup
{
    public class Relation : IEnumerable
    {
        public readonly long NumberOfRecords;
        public readonly string TableName;

        /* Creates an iterable factory that returns exactly the number
           of records specified. */
        public Relation(string tableName, long numberOfRecords)
        {
            Ensure.That(tableName).IsNotNullOrWhiteSpace();

            TableName = tableName;
            NumberOfRecords = numberOfRecords;
        }

        public static void ShowRecords(long numberOfRecords)
        {
            foreach (Record r in new Relation("test", numberOfRecords))
            {
                Console.WriteLine(r.CsvString);
            }
        }

        public void WriteCsv(string csvFilename)
        {
            using (StreamWriter sw = File.CreateText(csvFilename))
            {
                // Though the CLR uses UTF-16, CreateText opens a UTF-8 stream.

                foreach (Record record in this)
                {
                    // Write each record to the file on its own line.
                    sw.WriteLine(record.CsvString);
                }
            }
        }

        /* For IEnumerable. */
        public IEnumerator GetEnumerator()
        {
            return new RelationEnum(NumberOfRecords);
        }

        /* IEnumerator child-class required by IEnumerable. */
        private class RelationEnum : IEnumerator
        {
            private readonly long _numberOfRecords;
            private readonly long _prime, _generator;
            private long _seed;
            private long _currentIndex;

            public RelationEnum(long numberOfValuesToGenerate)
            {
                _numberOfRecords = numberOfValuesToGenerate;

                /* Open question: is there any reason not to just use the last one?! */
                if (_numberOfRecords <= 1000) { _generator = 279; _prime = 1009; }
                else if (_numberOfRecords <= 10000)
                {
                    _generator = 2969; _prime = 10007;
                }
                else if (_numberOfRecords <= 100000)
                {
                    _generator = 21395; _prime = 100003;
                }
                else if (_numberOfRecords <= 1000000)
                {
                    _generator = 2107;
                    _prime = 1000003;
                }
                else if (_numberOfRecords <= 10000000)
                {
                    _generator = 211;
                    _prime = 10000019;
                }
                else if (_numberOfRecords <= 100000000)
                {
                    _generator = 21;
                    _prime = 100000007;
                }
                else
                {
                    throw new ArgumentException("max must be less than or equal to 100,000,000.");
                }

                Reset();
            }

            public bool MoveNext()
            {
                long rand()
                {
                    do
                    {
                        /* I AM VERY SKEPTICAL OF THIS! WILL IT EVER GENERATE 0?!
                           Answer: no, but they subtract 1 to deal with that.

                           And, of course, will it actually generate unique values? */
                        _seed = (_generator * _seed) % _prime;
                    } while (
                        _seed > (_numberOfRecords)
                    );

                    return _seed;
                }

                if (_currentIndex >= _numberOfRecords - 1) // Since _currentIndex has to start at -1 for IEnumerable.
                {
                    return false;
                }

                rand();
                _currentIndex++;
                return true;
            }

            public void Reset()
            {
                _seed = _generator;
                _currentIndex = -1;
            }

            public object Current => new Record(_seed - 1, _currentIndex);
        }
    }
}
