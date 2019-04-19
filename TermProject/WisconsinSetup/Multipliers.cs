using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisconsinSetup
{
    class Multipliers : ObservableCollection<String>
    {
        public static System.Collections.Generic.Dictionary<string, long> Mappings { get; } = new Dictionary<string, long>();
        static Multipliers()
        {
            // Create the default mappings.
            _add("1x", 1);
            _add("Thousand", 1000);
            _add("Million", 1000000);
        }

        public Multipliers()
        {
            foreach (string key in Mappings.Keys)
            {
                Add(key);
            }
        }

        private static void _add(string key, long val)
        {
            Mappings.Add(key, val);
        }
    }
}
