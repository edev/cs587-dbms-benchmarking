using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsureThat;

namespace WisconsinSetup
{
    public class Record
    {
        // Construct our "xxx..." fills from an easily-verifiable base value of 5 x's.
        private const string XFill5 = "xxxxx";
        private const string XFill10 = XFill5 + XFill5;
        public const string XFill45 = XFill10 + XFill10 + XFill10 + XFill10 + XFill5;
        public const string XFill48 = XFill45 + "xxxx";

        public static readonly string[] String4Strings =  
        {
            "AAAA" + XFill48,
            "HHHH" + XFill48,
            "OOOO" + XFill48,
            "VVVV" + XFill48
        };

        public readonly long unique1,
            unique2,
            two,
            four,
            ten,
            twenty,
            onePercent,
            tenPercent,
            twentyPercent,
            fiftyPercent,
            unique3,
            evenOnePercent,
            oddOnePercent;

        // Beware: these are UTF-16!
        public readonly string stringu1, stringu2, string4;

        public string InsertString => "(" + CsvString + ")";

        public string CsvString =>
                String.Join(",", new string[]
                {
                    unique1.ToString(),
                    unique2.ToString(),
                    two.ToString(),
                    four.ToString(),
                    ten.ToString(),
                    twenty.ToString(),
                    onePercent.ToString(),
                    tenPercent.ToString(),
                    twentyPercent.ToString(),
                    fiftyPercent.ToString(),
                    unique3.ToString(),
                    evenOnePercent.ToString(),
                    oddOnePercent.ToString(),
                    stringu1,
                    stringu2,
                    string4
                });

        public Record(long unique1, long unique2)
        {
            this.unique1 = unique1;
            this.unique2 = unique2;
            two = unique1 % 2;
            four = unique1 % 4;
            ten = unique1 % 10;
            twenty = unique1 % 20;
            onePercent = unique1 % 100;
            tenPercent = unique1 % 10;
            twentyPercent = unique1 % 5;
            fiftyPercent = unique1 % 2;
            unique3 = unique1;
            evenOnePercent = onePercent * 2;
            oddOnePercent = evenOnePercent + 1;
            stringu1 = ConvertLongToString(unique1);
            stringu2 = ConvertLongToString(unique2);
            string4 = String4Strings[unique2 % String4Strings.Length];
        }

        public static string ConvertLongToString(long unique)
        {
            char[] tmp = new char[7];
            StringBuilder result = new StringBuilder(
                "AAAAAAA" + XFill45);

            int i;

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
            
            Ensure.That(result.Length).Is(52);
            return result.ToString();
        }
    }
}
