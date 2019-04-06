using System;
using System.Collections.Generic;
using System.Text;

namespace Emitter.Utility
{
    /*
    /// <summary>
    /// An entry in a dictionary from K to V.
    /// </summary>
    public struct Option
    {
        /// <summary>
        /// The key field of the entry
        /// </summary>
        public string Key;

        /// <summary>
        /// The value field of the entry
        /// </summary>
        public string Value;

        /// <summary>
        /// Create an entry with specified key and value
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public Option(string key, string value) { Key = key; Value = value; }

        /// <summary>
        /// Create an entry with a specified key. The value will be the default value of type <code>V</code>.
        /// </summary>
        /// <param name="key">The key</param>
        public Option(string key) { Key = key; Value = default(string); }

        /// <summary>
        /// Pretty print an entry
        /// </summary>
        /// <returns>(key, value)</returns>
        public override string ToString() { return Key + "=" + Value; }
    }*/

    public static class Options
    {
        public const string Retain = "+r";
        public const string QoS0 = "+0";
        public const string QoS1 = "+1";

        public static string WithTTL(int ttl) { return "ttl=" + ttl.ToString(); }

        public static string WithoutEcho() { return "me=" + "0"; }

        public static string WithLast(int last) { return "last=" + last.ToString(); }

        public static string WithRetain() { return Retain; }
    }
}
