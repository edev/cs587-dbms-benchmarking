using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;

namespace WisconsinSetup
{
    public class Relation : IEnumerable
    {
        private readonly long _numberOfRecords;

        /* Creates an iterable factory that returns exactly the number
           of records specified. */
        public Relation(long numberOfRecords)
        {
            _numberOfRecords = numberOfRecords;
        }

        public static string OrderedConvert(long unique)
        {
            char[] tmp = new char[7];
            char[] result = new char[7];
            long i;

            /* Set the result string to 'AAAAAA' initially. */
            for (i = 0; i < 7; i++)
            {
                result[i] = 'A';
            }

            /* Convert unique values from right to left into an alphabetic string in tmp */
            /* temp digits are right-justified in tmp */
            i = 6;
            while (unique > 0)
            {
                long remainder = unique % 26;
                tmp[i] = (char)('A' + remainder);
                unique /= 26;
                i--;
            }

            /* Finally, move tmp into result, left justifying it. */
            for (i = i + 1; i <= 6; i++)
            {
                result[i] = tmp[i];
            }
            return new string(result);
        }

        /* This class enumerates a fixed-length sequence of unique1 field values,
           i.e. values from 0 through MAX-1 in random order, where MAX is the length. */
        private class Unique1Enumerator : IEnumerable
        {
            private readonly long Max;

            public Unique1Enumerator(long max)
            {
                Max = max;
            }
            public IEnumerator GetEnumerator()
            {
                return new Unique1Enum(Max);
            }
        }

        private class Unique1Enum : IEnumerator
        {
            private readonly long _limit;
            private readonly long _prime, _generator;
            private long _seed;
            private long _currentIndex = 0;

            public Unique1Enum(long numberOfValuesToGenerate)
            {
                _limit = numberOfValuesToGenerate;

                /* Open question: is there any reason not to just use the last one?! */
                if (_limit <= 1000) { _generator = 279; _prime = 1009; }
                else if (_limit <= 10000)
                {
                    _generator = 2969; _prime = 10007;
                }
                else if (_limit <= 100000)
                {
                    _generator = 21395; _prime = 100003;
                }
                else if (_limit <= 1000000)
                {
                    _generator = 2107;
                    _prime = 1000003;
                }
                else if (_limit <= 10000000)
                {
                    _generator = 211;
                    _prime = 10000019;
                }
                else if (_limit <= 100000000)
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
                        _seed > (_limit)
                    );

                    return _seed;
                }

                if (_currentIndex >= _limit)
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
                _currentIndex = 0;
            }

            public object Current => _seed - 1;
        }

        public static void ShowUnique1Range(long numberOfValues)
        {
            foreach (long i in new Unique1Enumerator(numberOfValues))
            {
                Console.WriteLine(i);
            }
        }

        /* For IEnumerable. */
        public IEnumerator GetEnumerator()
        {
            return new DataFactoryEnum(_numberOfRecords);
        }

        /* IEnumerator child-class required by IEnumerable. */
        private class DataFactoryEnum : IEnumerator
        {
            private readonly long _numberOfRecords;
            private long currentIndex;

            public DataFactoryEnum(long numberOfRecords)
            {
                _numberOfRecords = numberOfRecords;
            }

            public bool MoveNext()
            {
                if (currentIndex >= _numberOfRecords)
                {
                    return false;
                }

                currentIndex++;
            }

            public void Reset()
            {
                currentIndex = 0;
            }

            public object Current => false;
        }
    }
}
