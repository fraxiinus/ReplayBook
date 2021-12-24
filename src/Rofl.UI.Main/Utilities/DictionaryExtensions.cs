using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rofl.UI.Main.Utilities
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Attempts to get boolean value from object dictionary
        /// </summary>
        /// <param name="values"></param>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetBool(this Dictionary<string, object> values, string key, out bool result)
        {
            if (values == null)
            {
                result = false;
                return false;
            }

            if (values.TryGetValue(key, out object rawObj))
            {
                if (rawObj == null)
                {
                    result = false;
                    return false;
                }

                try
                {
                    result = Convert.ToBoolean(rawObj, CultureInfo.InvariantCulture);
                    return true;
                }
                catch (FormatException)
                {
                    result = false;
                    return false;
                }
                catch (InvalidCastException)
                {
                    result = false;
                    return false;
                }
            }
            else
            {
                result = false;
                return false;
            }
        }

        /// <summary>
        /// Attempts to get string value from object dictionary
        /// </summary>
        /// <param name="values"></param>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetString(this Dictionary<string, object> values, string key, out string result)
        {
            if (values == null)
            {
                result = null;
                return false;
            }

            if (values.TryGetValue(key, out object rawObj))
            {
                try
                {
                    result = (string)rawObj;
                    return true;
                }
                catch (InvalidCastException)
                {
                    result = null;
                    return false;
                }
            }
            else
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Attempts to get double value from object dictionary
        /// </summary>
        /// <param name="values"></param>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetDouble(this Dictionary<string, object> values, string key, out double result)
        {
            if (values == null)
            {
                result = 0d;
                return false;
            }

            if (values.TryGetValue(key, out object rawObj))
            {
                if (rawObj == null)
                {
                    result = 0d;
                    return false;
                }

                try
                {
                    result = Convert.ToDouble(rawObj, CultureInfo.InvariantCulture);
                    return true;
                }
                catch (FormatException)
                {
                    // rawObj is not appropriate format for double
                    result = 0d;
                    return false;
                }
                catch (InvalidCastException)
                {
                    // rawObj does not implement IConvertible
                    result = 0d;
                    return false;
                }
                catch (OverflowException)
                {
                    result = 0d;
                    return false;
                }
            }
            else
            {
                result = 0d;
                return false;
            }
        }
    }
}
