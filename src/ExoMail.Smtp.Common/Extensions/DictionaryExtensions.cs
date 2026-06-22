using System;
using System.Collections.Generic;

namespace ExoMail.Smtp.Common.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool TryGetValue(this Dictionary<string, string> dictionary, string key, out int value)
        {
            string inVal;
            value = 0;
            if (dictionary.TryGetValue(key, out inVal))
                if (Int32.TryParse(inVal, out value))
                    return true;

            return false;
        }

        public static bool TryGetValue(this Dictionary<string, string> dictionary, string key, out string value, bool allowNullValue)
        {
            bool result = dictionary.TryGetValue(key, out value);

            if (!allowNullValue)
                return !String.IsNullOrEmpty(value) && result;
            else
                return result;
        }
    }
}