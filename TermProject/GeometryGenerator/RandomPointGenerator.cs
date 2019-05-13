using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryGenerator
{
    class RandomPointGenerator : IEnumerable
    {
        public readonly double xmin, xmax, ymin, ymax, interval, jitter;
        public RandomPointGenerator(double xmin, double xmax, double ymin, double ymax, double interval, double jitter)
        {
            this.xmin = xmin;
            this.xmax = xmax;
            this.ymin = ymin;
            this.ymax = ymax;
            this.interval = interval;
            this.jitter = jitter;
        }

        public IEnumerator GetEnumerator()
        {
            return new RandomPointGeneratorEnum(xmin, xmax, ymin, ymax, interval, jitter);
        }
    }

    class RandomPointGeneratorEnum : IEnumerator
    {
        public readonly double xmin, xmax, ymin, ymax, interval, jitter;
        private double _x, _y;
        private double _randomX, _randomY;
        private Random _rng = new Random();

        public RandomPointGeneratorEnum(double xmin, double xmax, double ymin, double ymax, double interval, double jitter)
        {
            this.xmin = xmin;
            this.xmax = xmax;
            this.ymin = ymin;
            this.ymax = ymax;
            this.interval = interval;
            this.jitter = jitter;
        }

        public object Current => new Point(_randomX, _randomY);

        public bool MoveNext()
        {
            // Increment x; if we overshoot xmax, then go to the next row.
            _x += interval;
            if (_x > xmax)
            {
                _x = xmin;
                _y += interval;
            }

            // If we just overshot ymax, then we're done.
            if (_y > ymax)
            {
                return false;
            }

            // We're still in-bounds.
            _randomX = _randomize(_x, jitter);
            _randomY = _randomize(_y, jitter);
            return true;
        }

        public void Reset()
        {
            _x = xmin;
            _y = ymin;
            _randomX = _randomize(_x, jitter);
            _randomY = _randomize(_y, jitter);
        }

        /// <summary>
        /// Generates a randomized value in the window [start - jitter, start + jitter).
        /// </summary>
        /// <param name="start">The starting value, i.e. the center of the random range.</param>
        /// <param name="jitter">The maximum amount that may be added to or subtracted from start.</param>
        /// <returns></returns>
        private double _randomize(double start, double jitter)
        {
            var rng = _rng.NextDouble();

            // Generate a random number in the range [-1, 1)
            double offset = (rng - 0.5) * 2;

            // Scale by jitter.
            offset = offset * jitter;

            return start + offset;
        }
    }
    
    class Point
    {
        public readonly double X;
        public readonly double Y;

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
