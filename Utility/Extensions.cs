using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Utility
{
    public static class Extensions
    {
        private static readonly string[] NumericWords =
        {
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine",
            "ten",
            "eleven",
            "twelve",
            "thirteen",
            "fourteen",
            "fifteen",
            "sixteen",
            "seventeen",
            "eighteen",
            "nineteen"
        };

        private static readonly string[] DecadeWords =
        {
            "twenty",
            "thirty",
            "forty",
            "fifty",
            "sixty",
            "seventy",
            "eighty",
            "ninety"
        };

        public static Window GetParentWindow(this DependencyObject extended)
        {
            var parent = VisualTreeHelper.GetParent(extended);

            if (parent == null)
            {
                return null;
            }

            if (parent is Window)
            {
                return ((Window) parent);
            }

            return parent.GetParentWindow();
        }

        public static string RemoveIlegalCharacters(this string extended)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return new string(extended.Where(x => !invalidChars.Contains(x)).ToArray());
        }

        public static bool IsValid(this DependencyObject obj)
        {
            // The dependency object is valid if it has no errors and all
            // of its children (that are dependency objects) are error-free.
            return !Validation.GetHasError(obj) &&
                   LogicalTreeHelper.GetChildren(obj).OfType<DependencyObject>().All(IsValid);
        }

        public static string Stuff(this string extended, int index, int length, string insert)
        {
            var value = extended;
            value = value.Remove(index, length);
            return value.Insert(index, insert);
        }

        /// <summary>
        ///     Adds the ordinal suffix to an integer
        /// </summary>
        /// <param name="extended">The integer to add the suffix to</param>
        /// <returns>The orginal with it's appropriate suffix</returns>
        /// <remarks>Currently only supports English</remarks>
        public static string ToOrdinal(this int extended)
        {
            var lastDigit = int.Parse(extended.ToString().Substring(extended.ToString().Length - 1, 1));

            if (extended <= 0)
            {
                return extended.ToString();
            }

            switch (extended%100)
            {
                case 11:
                case 12:
                case 13:
                    return extended + "th";
            }

            switch (lastDigit)
            {
                case 1:
                    return extended + "st";
                case 2:
                    return extended + "nd";
                case 3:
                    return extended + "rd";
                default:
                    return extended + "th";
            }
        }

        /// <summary>
        ///     Converts an integer to its numeric word
        /// </summary>
        /// <param name="extended">The integer to convert</param>
        /// <returns>The numeric word for the integer</returns>
        /// <remarks>Currently only supports English</remarks>
        public static string ToText(this int extended)
        {
            if (extended < 0)
            {
                return "negative " + ToText(-extended);
            }
            if (extended == 0)
            {
                return "";
            }
            if (extended <= 19)
            {
                return NumericWords[extended - 1] + " ";
            }
            if (extended <= 99)
            {
                return DecadeWords[extended/10 - 2] + " " + ToText(extended%10);
            }
            if (extended <= 999)
            {
                return ToText(extended/100) + "hundred " + ToText(extended%100);
            }
            if (extended <= 999999)
            {
                return ToText(extended/1000) + "thousand " + ToText(extended%1000);
            }
            if (extended <= 999999999)
            {
                return ToText(extended/1000000) + "million " + ToText(extended%1000000);
            }
            return ToText(extended/1000000000) + "billion " + ToText(extended%1000000000);
        }

        public static void AppendFormatLine(this StringBuilder extended, string format, params object[] args)
        {
            extended.AppendFormat(format, args);
            extended.AppendLine();
        }

        public static void AddRange(this List<string> extended, StringCollection source)
        {
            foreach (var item in source)
            {
                extended.Add(item);
            }
        }

        public static StringCollection ToStringCollection(this List<string> extended)
        {
            var value = new StringCollection();
            foreach (var item in extended)
            {
                value.Add(item);
            }
            return value;
        }

        public static List<string> ToList(this StringCollection extended)
        {
            var value = new List<string>();
            foreach (var item in extended)
            {
                value.Add(item);
            }
            return value;
        }

        /// <summary>
        ///     Does basic validation of a filename or path
        /// </summary>
        /// <param name="extended">The string to validate</param>
        /// <returns>True if basic validation is passed; False otherwise</returns>
        public static bool CheckPathForInvalidCharacters(this string extended)
        {
            if (string.IsNullOrWhiteSpace(extended))
            {
                return false;
            }

            foreach (var badChar in Path.GetInvalidPathChars())
            {
                if (extended.Contains(badChar.ToString()))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Returns the <paramref>extended</paramref> string repeated as many times as indictated
        ///     by the <paramref>count</paramref> parameter
        /// </summary>
        /// <param name="extended">The string to repeat</param>
        /// <param name="count">The number of times to repeat <paramref>extended</paramref></param>
        /// <returns>The extended string repeated <paramref>count</paramref> times</returns>
        public static string Repeat(this string extended, int count)
        {
            var value = string.Empty;
            if (count > 0)
            {
                for (var i = 1; i <= count; i++)
                {
                    value += extended;
                }
            }
            else
            {
                throw (new ArgumentException("Value must be greater than 0", "count"));
            }
            return value;
        }

        public static bool In<T>(this T extended, params T[] values)
        {
            if (typeof (T) == typeof (string))
            {
                var stringValues = Array.ConvertAll(values, x => Convert.ToString(x));
                return extended.ToString().In(StringComparison.InvariantCultureIgnoreCase, stringValues);
            }

            foreach (var item in values)
            {
                if (extended.Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool In(this string extended, StringComparison comparison, params string[] values)
        {
            foreach (var value in values)
            {
                if (extended.Equals(value, comparison))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsAlphaNumeric(this string extended)
        {
            return extended.IsAlphaNumeric(false);
        }

        public static bool IsAlphaNumeric(this string extended, bool allowWhitespace)
        {
            return !extended.ToArray()
                            .Where(c => !char.IsLetterOrDigit(c) && !(allowWhitespace && char.IsWhiteSpace(c)))
                            .Any();
        }

        public static string Remove(this string extended, params string[] items)
        {
            return items.Aggregate(extended, (current, item) => current.Replace(item, string.Empty));
        }

        public static int Val(this string extended)
        {
            return Convert.ToInt32(extended.Val(false));
        }

        public static double Val(this string extended, bool allowDecimal)
        {
            var decimalFound = false;
            var signFound = false;
            var value = string.Empty;

            foreach (var c in extended)
            {
                if (char.IsNumber(c))
                {
                    value += c.ToString();
                }
                else if (allowDecimal && !decimalFound && c == '.')
                {
                    value += c.ToString();
                    decimalFound = true;
                }
                else if (!signFound && "+-".Contains(c.ToString()))
                {
                    value += c.ToString();
                    signFound = true;
                }
            }
            return double.Parse(value);
        }

        public static PropertyInfo GetProperty(this PropertyInfo[] extended, string name, bool ignoreCase)
        {
            if (ignoreCase)
            {
                return extended.FirstOrDefault(p => p.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            }
            return extended.FirstOrDefault(p => p.Name.Equals(name));
        }
    }
}