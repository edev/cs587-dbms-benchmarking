using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisconsinSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            // convertRange(Relation.OrderedConvert, 0, 32);

            // long start = 0;
            // long end = 47995000;
            //testRange(Relation.KavinConvert, start, end);
            //testRange(Relation.Convert, start, end);
            // testRange(Relation.PaperConvert, start, end, true);
            // testRange(Relation.OrderedConvert, start, end); // Good! No duplicates found.

            Relation.ShowUnique1Range(64);

            Console.ReadLine();
        }

        static void convertRange(Func<long, string> converter, long start, long end)
        {
            for (long i = start; i <= end; i++)
            {
                convert(converter, i);
            }
        }

        static void convert(Func<long, string> converter, long number)
        {
            Console.WriteLine($"{number}\t->  {converter(number)}");
        }

        static Boolean testRange(Func<long, string> converter, long start, long end, Boolean continueOnDuplicate=false)
        {
            Dictionary<string, long> table = new Dictionary<string, long>();
            long duplicatesFound = 0;

            for (long i = start; i <= end; i++)
            {
                string output = converter(i);
                if (table.ContainsKey(output))
                {
                    Console.WriteLine($"Duplicate found! Both {table[output]} and {i} map to {output}");
                    duplicatesFound++;
                    if (continueOnDuplicate == false)
                    {
                        return false;
                    }
                }
                else
                {
                    table.Add(output, i);
                }
            }
            if (duplicatesFound == 0)
            {
                Console.WriteLine("No duplicates found.");
            }
            else
            {
                Console.WriteLine($"Found {duplicatesFound} duplicates.");
            }
            return true;
        }
    }
}
