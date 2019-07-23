using System;
using System.Collections.Generic;

namespace Rofl.Reader.Utilities
{
    public static class DictionaryExtensions
    {
        public static String SafeGet(this Dictionary<string, string> keyValues, string key)
        {
            string returnVal = "0";
            try
            {
                returnVal = keyValues[key];
            }
            catch (Exception)
            { }

            return returnVal;
        }
    }
}
