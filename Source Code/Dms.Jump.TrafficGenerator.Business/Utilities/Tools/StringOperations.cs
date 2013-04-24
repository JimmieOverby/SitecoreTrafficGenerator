using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dms.Jump.TrafficGenerator.Business.Utilities.Tools
{
    public static class StringOperations
    {
        public static List<string> SplitStringToEnumerable(string delimitedString, char delimiter)
        {
            var stringCollection = new List<string>();
            if (!String.IsNullOrEmpty(delimitedString))
            {
                var strignArray = delimitedString.Split(delimiter);
                stringCollection = strignArray.ToList<string>();
            }

            return stringCollection;
        }

        public static int GetRandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        /// <summary>
        /// Test for IP address.
        /// </summary>
        public static bool IsIP(string strToCheck, bool isRequiredField)
        {
            if (!isRequiredField && strToCheck.Length <= 0)
                return true;
            else if (isRequiredField && strToCheck.Length <= 0)
                return false;

            Regex objIPPattern =
                new Regex(
                    @"(?<First>2[0-4]\d|25[0-5]|[01]?\d\d?)\." +
                        @"(?<Second>2[0-4]\d|25[0-5]|[01]?\d\d?)\." +
                        @"(?<Third>2[0-4]\d|25[0-5]|[01]?\d\d?)\." +
                        @"(?<Fourth>2[0-4]\d|25[0-5]|[01]?\d\d?)");

            return objIPPattern.IsMatch(strToCheck);
        }

    }
}
