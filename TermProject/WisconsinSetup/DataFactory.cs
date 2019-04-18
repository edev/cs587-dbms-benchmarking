using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;

namespace WisconsinSetup
{
    public class DataFactory
    {

        public static string PaperConvert(long unique)
        {
            char[] tmp = new char[7];
            char[] result = new char[7];
            long i, j, remainder, count;

            /* Set the result string to 'AAAAAA' initially. */
            for (i = 0; i < 7; i++)
            {
                result[i] = 'A';
            }

            /* Convert unique values from right to left into an alphabetic string in tmp */
            /* temp digits are right-justified in tmp */
            i = 6;
            count = 0;
            while (unique > 0)
            {
                remainder = unique % 26;
                tmp[i] = (char) ('A' + remainder);
                unique /= 26;
                i--;
                count++;
            }

            /* FIXED: increment i beforehand. */
            i++;

            /* Finally, move tmp into result, left justifying it. */
            /* FIXED: j < count. */
            for (j = 0; j < count; j++, i++)
            {
                result[j] = tmp[i];
            }
            return new string(result);
        }

        public static string KavinConvert(long unique)
        {
            char[] tmp = new char[7];
            char[] result = new char[7];
            long i, j, remainder, count;

            /* Set the result string to 'AAAAAA' initially. */
            for (i = 0; i < 7; i++)
            {
                result[i] = 'A';
            }

            /* Convert unique values from right to left into an alphabetic string in tmp */
            /* temp digits are right-justified in tmp */
            i = 6;
            count = 0;
            while (unique > 0)
            {
                remainder = unique % 26;
                tmp[i] = (char)('A' + remainder);
                unique /= 26;
                i--;
                count++;
            }

            /* Finally, move tmp into result, left justifying it. */
            for (j = 0; j < count; j++, i++)
            {
                result[j] = tmp[6-j];
            }
            return new string(result);
        }

        public static string SpaceConvert(long unique)
        {
            char[] tmp = new char[7];
            char[] result = new char[7];
            long i, j, remainder, count;

            /* Set the result string to 'AAAAAA' initially. */
            for (i = 0; i < 7; i++)
            {
                result[i] = ' ';
            }

            /* Convert unique values from right to left into an alphabetic string in tmp */
            /* temp digits are right-justified in tmp */
            i = 6;
            count = 0;
            while (unique > 0)
            {
                remainder = unique % 26;
                tmp[i] = (char)('A' + remainder);
                unique /= 26;
                i--;
                count++;
            }

            /* We went one index too far with i, so we need to correct. */
            i++;

            /* Finally, move tmp into result, left justifying it. */
            for (j = 0; j < count; j++, i++)
            {
                result[j] = tmp[i];
            }
            return new string(result);
        }

        public static string OrderedConvert(long unique)
        {
            char[] tmp = new char[7];
            char[] result = new char[7];
            long i, remainder;

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
                remainder = unique % 26;
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
    }
}
