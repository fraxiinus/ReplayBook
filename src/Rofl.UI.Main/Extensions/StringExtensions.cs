using System;
using System.Globalization;
using System.Windows.Media;

namespace Rofl.UI.Main.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// https://stackoverflow.com/questions/17563929/how-to-make-string-contains-case-insensitive/17563994#17563994
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toCheck"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source != null && toCheck != null && source.Contains(toCheck, comp);
        }

        /// <summary>
        /// 0 if null, -1 if invalid
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int ToInt(this string source)
        {
            if (source == null) { return 0; }

            int result = -1;
            if (int.TryParse(source, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parseResult))
            {
                result = parseResult;
            }

            return result;
        }

        public static string ToHexString(this Color c)
        {
            return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }
    }
}
