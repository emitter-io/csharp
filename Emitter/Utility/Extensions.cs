/*
Copyright (c) 2016 Roman Atachiants
Copyright (c) 2013, 2014 Paolo Patierno

All rights reserved. This program and the accompanying materials
are made available under the terms of the Eclipse Public License v1.0
and Eclipse Distribution License v1.0 which accompany this distribution.

The Eclipse Public License:  http://www.eclipse.org/legal/epl-v10.html
The Eclipse Distribution License: http://www.eclipse.org/org/documents/edl-v10.php.

Contributors:
   Paolo Patierno - initial API and implementation and/or initial documentation
   Roman Atachiants - integrating with emitter.io
*/

using System.Collections;
using System.Globalization;
using System.Threading;
using Emitter;

#if (MF)
using Microsoft.SPOT;
#endif
#if WINRT

using System.Linq;
using System.Reflection;

#endif

namespace System
{
    /// <summary>
    /// Extension class for a Queue
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Predicate for searching inside a queue
        /// </summary>
        /// <param name="item">Item of the queue</param>
        /// <returns>Result of predicate</returns>
        internal delegate bool QueuePredicate(object item);

        /// <summary>
        /// Get (without removing) an item from queue based on predicate
        /// </summary>
        /// <param name="queue">Queue in which to search</param>
        /// <param name="predicate">Predicate to verify to get item</param>
        /// <returns>Item matches the predicate</returns>
        internal static object GetFromQueue(Queue queue, QueuePredicate predicate)
        {
            foreach (var item in queue)
            {
                if (predicate(item))
                    return item;
            }
            return null;
        }

#if WINRT

        /// <summary>
        /// Gets the methods from the type.
        /// </summary>
        /// <param name="type">The type to retrieve the methods from.</param>
        /// <returns></returns>
        public static MethodInfo[] GetMethods(this Type type)
        {
            return type.GetTypeInfo().DeclaredMethods.ToArray();
        }

#endif

#if MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3
        /// <summary>
        /// Gets whether the hashtable contains the key.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static bool ContainsKey(this Hashtable source, object key)
        {
            return source.Contains(key);
        }

        /// <summary>
        /// Determines whether the end of this string instance matches the specified string.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value">The string to compare to the substring at the end of this instance.</param>
        /// <returns>true if value matches the end of this instance; otherwise, false.</returns>
        public static bool EndsWith(this string s, string value)
        {
            return s.IndexOf(value) == s.Length - value.Length;
        }

        /// <summary>
        /// Determines whether the beginning of this string instance matches the specified string.
        /// </summary>
        /// <param name="s">The string to compare.</param>
        /// <param name="value">The string to compare.</param>
        /// <returns>true if value matches the beginning of this string; otherwise, false.</returns>
        public static bool StartsWith(this string s, string value)
        {
            return s.IndexOf(value) == 0;
        }

        /// <summary>
        ///  Returns a value indicating whether a specified substring occurs within this string.
        /// </summary>
        /// <param name="s">The string to check.</param>
        /// <param name="value">The string to seek.</param>
        /// <returns> true if the value parameter occurs within this string, or if value is the empty
        /// string (""); otherwise, false.</returns>
        public static bool Contains(this string s, string value)
        {
            return s.IndexOf(value) >= 0;
        }
#endif

        /// <summary>
        /// Attempts to parse a number into a Uint32
        /// </summary>
        /// <param name="str"></param>
        /// <param name="style"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseUInt32(string str, NumberEncoding style, out UInt32 result)
        {
            bool sign;
            ulong tmp;

            bool bresult = Helper.TryParseUInt64Core(str, style == NumberEncoding.Hexadecimal ? true : false, out tmp, out sign);
            result = (UInt32)tmp;

            return bresult && !sign;
        }

        /// <summary>
        /// Converts a Unicode character to a string of its ASCII equivalent.
        /// Very simple, it works only on ordinary characters.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string ConvertFromUtf32(int p)
        {
            char c = (char)p;
            return c.ToString();
        }

        public static long ParseInt64(string str)
        {
            long result;
            if (TryParseInt64(str, out result))
            {
                return result;
            }
            throw new Exception();
        }

        public static long ParseInt64(string str, NumberEncoding style)
        {
            if (style == NumberEncoding.Hexadecimal)
            {
                return ParseInt64Hex(str);
            }

            return ParseInt64(str);
        }

        public static bool TryParseInt64(string str, out long result)
        {
            result = 0;
            ulong r;
            bool sign;
            if (Helper.TryParseUInt64Core(str, false, out r, out sign))
            {
                if (!sign)
                {
                    if (r <= 9223372036854775807)
                    {
                        result = unchecked((long)r);
                        return true;
                    }
                }
                else
                {
                    if (r <= 9223372036854775808)
                    {
                        result = unchecked(-((long)r));
                        return true;
                    }
                }
            }
            return false;
        }

        private static long ParseInt64Hex(string str)
        {
            ulong result;
            if (TryParseInt64Hex(str, out result))
            {
                return (long)result;
            }
            throw new Exception();
        }

        private static bool TryParseInt64Hex(string str, out ulong result)
        {
            bool sign;
            return Helper.TryParseUInt64Core(str, true, out result, out sign);
        }

        /// <summary>
        /// Converts an ISO 8601 time/date format string, which is used by JSON and others,
        /// into a DateTime object.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime FromIso8601(string date)
        {
            // Check to see if format contains the timezone ID, or contains UTC reference
            // Neither means it's localtime
            bool utc = date.EndsWith("Z");

            string[] parts = date.Split(new char[] { 'T', 'Z', ':', '-', '.', '+', });

            // We now have the time string to parse, and we'll adjust
            // to UTC or timezone after parsing
            string year = parts[0];
            string month = (parts.Length > 1) ? parts[1] : "1";
            string day = (parts.Length > 2) ? parts[2] : "1";
            string hour = (parts.Length > 3) ? parts[3] : "0";
            string minute = (parts.Length > 4) ? parts[4] : "0";
            string second = (parts.Length > 5) ? parts[5] : "0";
            string ms = (parts.Length > 6) ? parts[6] : "0";

            DateTime dt = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day), Convert.ToInt32(hour), Convert.ToInt32(minute), Convert.ToInt32(second), Convert.ToInt32(ms));

            // If a time offset was specified instead of the UTC marker,
            // add/subtract in the hours/minutes
            if ((utc == false) && (parts.Length >= 9))
            {
                // There better be a timezone offset
                string hourOffset = (parts.Length > 7) ? parts[7] : "";
                string minuteOffset = (parts.Length > 8) ? parts[8] : "";
                if (date.Contains("+"))
                {
                    dt = dt.AddHours(Convert.ToDouble(hourOffset));
                    dt = dt.AddMinutes(Convert.ToDouble(minuteOffset));
                }
                else
                {
                    dt = dt.AddHours(-(Convert.ToDouble(hourOffset)));
                    dt = dt.AddMinutes(-(Convert.ToDouble(minuteOffset)));
                }
            }

            if (utc)
            {
                // Convert the Kind to DateTimeKind.Utc if string Z present
                dt = new DateTime(0, DateTimeKind.Utc).AddTicks(dt.Ticks);
            }

            return dt;
        }

        /// <summary>
        /// Converts a DateTime object into an ISO 8601 string.  This version
        /// always returns the string in UTC format.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToIso8601(DateTime dt)
        {
            string result = dt.Year.ToString() + "-" +
                            TwoDigits(dt.Month) + "-" +
                            TwoDigits(dt.Day) + "T" +
                            TwoDigits(dt.Hour) + ":" +
                            TwoDigits(dt.Minute) + ":" +
                            TwoDigits(dt.Second) + "." +
                            ThreeDigits(dt.Millisecond) + "Z";

            return result;
        }

        /// <summary>
        /// Ensures a two-digit number with leading zero if necessary.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string TwoDigits(int value)
        {
            if (value < 10)
            {
                return "0" + value.ToString();
            }

            return value.ToString();
        }

        /// <summary>
        /// Ensures a three-digit number with leading zeros if necessary.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ThreeDigits(int value)
        {
            if (value < 10)
            {
                return "00" + value.ToString();
            }
            else if (value < 100)
            {
                return "0" + value.ToString();
            }

            return value.ToString();
        }

        /// <summary>
        /// The ASP.NET Ajax team made up their own time date format for JSON strings, and it's
        /// explained in this article: http://msdn.microsoft.com/en-us/library/bb299886.aspx
        /// Converts a DateTime to the ASP.NET Ajax JSON format.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToASPNetAjax(DateTime dt)
        {
            string value = dt.Ticks.ToString();

            return @"\/Date(" + value + @")\/";
        }

        /// <summary>
        /// Converts an ASP.NET Ajax JSON string to DateTime
        /// </summary>
        /// <param name="ajax"></param>
        /// <returns></returns>
        public static DateTime FromASPNetAjax(string ajax)
        {
            var parts = ajax.Split(new char[] { '(', ')' });
            var ticks = Convert.ToInt64(parts[1]);
            return new DateTime(ticks, DateTimeKind.Utc);
        }

        /// <summary>
        /// Attempts to fetch a value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool TryGetValueFromHashtable(Hashtable source, object key, out object value)
        {
            value = null;
            Monitor.Enter(source);
            try
            {
                if (!source.ContainsKey(key))
                    return false;

                value = source[key];
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                Monitor.Exit(source);
            }
        }

        /// <summary>
        /// Gets or adds a value to the hashtable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static object GetOrAddToHashtable(Hashtable source, object key, object value)
        {
            Monitor.Enter(source);
            try
            {
                // Do we have it?
                if (source.ContainsKey(key))
                    return source[key];

                source[key] = value;
                return value;
            }
            finally
            {
                Monitor.Exit(source);
            }
        }

        #region Private Static Helper Methods

        internal static class Helper
        {
            public const int MaxDoubleDigits = 16;

            public static bool IsWhiteSpace(char ch)
            {
                return ch == ' ';
            }

            // Parse integer values using localized number format information.
            public static bool TryParseUInt64Core(string str, bool parseHex, out ulong result, out bool sign)
            {
                if (str == null)
                {
                    throw new ArgumentNullException("str");
                }

                // If number contains the Hex '0x' prefix, then make sure we're
                // managing a Hex number, and skip over the '0x'
                if (str.Length >= 2 && str.Substring(0, 2).ToLower() == "0x")
                {
                    str = str.Substring(2);
                    parseHex = true;
                }

                char ch;
                bool noOverflow = true;
                result = 0;

                // Skip leading white space.
                int len = str.Length;
                int posn = 0;
                while (posn < len && IsWhiteSpace(str[posn]))
                {
                    posn++;
                }

                // Check for leading sign information.
                NumberFormatInfo nfi = CultureInfo.CurrentUICulture.NumberFormat;
                string posSign = nfi.PositiveSign;
                string negSign = nfi.NegativeSign;
                sign = false;
                while (posn < len)
                {
                    ch = str[posn];
                    if (!parseHex && ch == negSign[0])
                    {
                        sign = true;
                        ++posn;
                    }
                    else if (!parseHex && ch == posSign[0])
                    {
                        sign = false;
                        ++posn;
                    }
                    /*      else if (ch == thousandsSep[0])
                            {
                                ++posn;
                            }*/
                    else if ((parseHex && ((ch >= 'A' && ch <= 'F') || (ch >= 'a' && ch <= 'f'))) ||
                             (ch >= '0' && ch <= '9'))
                    {
                        break;
                    }
                    else
                    {
                        return false;
                    }
                }

                // Bail out if the string is empty.
                if (posn >= len)
                {
                    return false;
                }

                // Parse the main part of the number.
                uint low = 0;
                uint high = 0;
                uint digit;
                ulong tempa, tempb;
                if (parseHex)
                {
                    #region Parse a hexadecimal value.
                    do
                    {
                        // Get the next digit from the string.
                        ch = str[posn];
                        if (ch >= '0' && ch <= '9')
                        {
                            digit = (uint)(ch - '0');
                        }
                        else if (ch >= 'A' && ch <= 'F')
                        {
                            digit = (uint)(ch - 'A' + 10);
                        }
                        else if (ch >= 'a' && ch <= 'f')
                        {
                            digit = (uint)(ch - 'a' + 10);
                        }
                        else
                        {
                            break;
                        }

                        // Combine the digit with the result, and check for overflow.
                        if (noOverflow)
                        {
                            tempa = ((ulong)low) * ((ulong)16);
                            tempb = ((ulong)high) * ((ulong)16);
                            tempb += (tempa >> 32);
                            if (tempb > ((ulong)0xFFFFFFFF))
                            {
                                // Overflow has occurred.
                                noOverflow = false;
                            }
                            else
                            {
                                tempa = (tempa & 0xFFFFFFFF) + ((ulong)digit);
                                tempb += (tempa >> 32);
                                if (tempb > ((ulong)0xFFFFFFFF))
                                {
                                    // Overflow has occurred.
                                    noOverflow = false;
                                }
                                else
                                {
                                    low = unchecked((uint)tempa);
                                    high = unchecked((uint)tempb);
                                }
                            }
                        }
                        ++posn; // Advance to the next character.
                    } while (posn < len);
                    #endregion Parse a hexadecimal value.
                }
                else
                {
                    #region Parse a decimal value.
                    do
                    {
                        // Get the next digit from the string.
                        ch = str[posn];
                        if (ch >= '0' && ch <= '9')
                        {
                            digit = (uint)(ch - '0');
                        }
                        /*       else if (ch == thousandsSep[0])
                               {
                                   // Ignore thousands separators in the string.
                                   ++posn;
                                   continue;
                               }*/
                        else
                        {
                            break;
                        }

                        // Combine the digit with the result, and check for overflow.
                        if (noOverflow)
                        {
                            tempa = ((ulong)low) * ((ulong)10);
                            tempb = ((ulong)high) * ((ulong)10);
                            tempb += (tempa >> 32);
                            if (tempb > ((ulong)0xFFFFFFFF))
                            {
                                // Overflow has occurred.
                                noOverflow = false;
                            }
                            else
                            {
                                tempa = (tempa & 0xFFFFFFFF) + ((ulong)digit);
                                tempb += (tempa >> 32);
                                if (tempb > ((ulong)0xFFFFFFFF))
                                {
                                    // Overflow has occurred.
                                    noOverflow = false;
                                }
                                else
                                {
                                    low = unchecked((uint)tempa);
                                    high = unchecked((uint)tempb);
                                }
                            }
                        }
                        ++posn;// Advance to the next character.
                    } while (posn < len);
                    #endregion Parse a decimal value.
                }

                // Process trailing white space.
                if (posn < len)
                {
                    do
                    {
                        ch = str[posn];
                        if (IsWhiteSpace(ch))
                            ++posn;
                        else
                            break;
                    } while (posn < len);

                    if (posn < len)
                    {
                        return false;
                    }
                }

                // Return the results to the caller.
                result = (((ulong)high) << 32) | ((ulong)low);
                return noOverflow;
            }
        }

        #endregion Private Static Helper Methods
    }

    /// <summary>
    /// Represents a number style.
    /// </summary>
    internal enum NumberEncoding
    {
        Decimal = 1,
        Hexadecimal
    }
}