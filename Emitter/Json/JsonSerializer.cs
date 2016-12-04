/*
Copyright (c) 2016 Roman Atachiants
Copyright (c) 2011-2013 Mike Jones, Matt Weimer

The MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files
(the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge,
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections;
using System.Reflection;
using System.Text;

#if (MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3)
using Microsoft.SPOT;
#endif

namespace Emitter
{
    /// <summary>
    /// JSON.NetMF - JSON Serialization and Deserialization library for .NET Micro Framework
    /// </summary>
    internal class JsonSerializer
    {
        public JsonSerializer(DateTimeFormat dateTimeFormat = DateTimeFormat.Default)
        {
            DateFormat = dateTimeFormat;
        }

        /// <summary>
        /// Gets/Sets the format that will be used to display
        /// and parse dates in the Json data.
        /// </summary>
        public DateTimeFormat DateFormat { get; set; }

        /// <summary>
        /// Convert an object to a JSON string.
        /// </summary>
        /// <param name="o">The value to convert. Supported types are: Boolean, String, Byte, (U)Int16, (U)Int32, Float, Double, Decimal, Array, IDictionary, IEnumerable, Guid, Datetime, DictionaryEntry, Object and null.</param>
        /// <returns>The JSON object as a string or null when the value type is not supported.</returns>
        /// <remarks>For objects, only public properties with getters are converted.</remarks>
		public string Serialize(object o)
        {
            return SerializeObject(o, this.DateFormat);
        }

        /// <summary>
        /// Desrializes a Json string into an object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns>An ArrayList, a Hashtable, a double, a long, a string, null, true, or false</returns>
        public object Deserialize(string json)
        {
            return DeserializeString(json);
        }

        /// <summary>
        /// Deserializes a Json string into an object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns>An ArrayList, a Hashtable, a double, a long, a string, null, true, or false</returns>
        public static object DeserializeString(string json)
        {
            return JsonParser.JsonDecode(json);
        }

        /// <summary>
        /// Convert an object to a JSON string.
        /// </summary>
        /// <param name="o">The value to convert. Supported types are: Boolean, String, Byte, (U)Int16, (U)Int32, Float, Double, Decimal, Array, IDictionary, IEnumerable, Guid, Datetime, DictionaryEntry, Object and null.</param>
        /// <returns>The JSON object as a string or null when the value type is not supported.</returns>
        /// <remarks>For objects, only public properties with getters are converted.</remarks>
        public static string SerializeObject(object o, DateTimeFormat dateTimeFormat = DateTimeFormat.Default)
        {
            if (o == null)
                return "null";

            Type type = o.GetType();

            switch (type.Name)
            {
                case "Boolean":
                    {
                        return (bool)o ? "true" : "false";
                    }
                case "String":
                case "Char":
                case "Guid":
                    {
                        return "\"" + SerializeString(o as String) + "\"";
                    }
                case "Single":
                case "Double":
                case "Decimal":
                case "Float":
                case "Byte":
                case "SByte":
                case "Int16":
                case "UInt16":
                case "Int32":
                case "UInt32":
                case "Int64":
                case "UInt64":
                    {
                        return o.ToString();
                    }
                case "DateTime":
                    {
                        switch (dateTimeFormat)
                        {
                            case DateTimeFormat.Ajax:
                                // This MSDN page describes the problem with JSON dates:
                                // http://msdn.microsoft.com/en-us/library/bb299886.aspx
                                return "\"" + Utils.ToASPNetAjax((DateTime)o) + "\"";

                            case DateTimeFormat.ISO8601:
                            case DateTimeFormat.Default:
                            default:
                                return "\"" + Utils.ToIso8601((DateTime)o) + "\"";
                        }
                    }
            }

            if (o is IDictionary && !type.IsArray)
            {
                IDictionary dictionary = o as IDictionary;
                return SerializeIDictionary(dictionary, dateTimeFormat);
            }

            if (o is IEnumerable)
            {
                IEnumerable enumerable = o as IEnumerable;
                return SerializeIEnumerable(enumerable, dateTimeFormat);
            }

            if (type == typeof(DictionaryEntry))
            {
                DictionaryEntry entry = (DictionaryEntry)o;
                Hashtable hashtable = new Hashtable();
                hashtable.Add(entry.Key, entry.Value);
                return SerializeIDictionary(hashtable, dateTimeFormat);
            }

#if (!FX && !MF)
            if (type.GetTypeInfo().IsClass)
#else
            if (type.IsClass)
#endif
            {
                Hashtable hashtable = new Hashtable();

                // Iterate through all of the methods, looking for public GET properties
#if (!FX && !MF && !WINRT)
                MethodInfo[] methods = type.GetTypeInfo().GetMethods();
#else
                MethodInfo[] methods = type.GetMethods();
#endif
                foreach (MethodInfo method in methods)
                {
                    // We care only about property getters when serializing
                    if (method.Name.StartsWith("get_"))
                    {
                        // Ignore abstract and virtual objects
                        if (method.IsAbstract)
                        {
                            continue;
                        }

                        // Ignore delegates and MethodInfos
                        if ((method.ReturnType == typeof(System.Delegate)) ||
                            (method.ReturnType == typeof(System.MulticastDelegate)) ||
                            (method.ReturnType == typeof(System.Reflection.MethodInfo)))
                        {
                            continue;
                        }
                        // Ditto for DeclaringType
                        if ((method.DeclaringType == typeof(System.Delegate)) ||
                            (method.DeclaringType == typeof(System.MulticastDelegate)))
                        {
                            continue;
                        }

                        object returnObject = method.Invoke(o, null);
                        hashtable.Add(method.Name.Substring(4), returnObject);
                    }
                }
                return SerializeIDictionary(hashtable, dateTimeFormat);
            }

            return null;
        }

        /// <summary>
        /// Convert an IEnumerable to a JSON string.
        /// </summary>
        /// <param name="enumerable">The value to convert.</param>
        /// <returns>The JSON object as a string or null when the value type is not supported.</returns>
        protected static string SerializeIEnumerable(IEnumerable enumerable, DateTimeFormat dateTimeFormat = DateTimeFormat.Default)
        {
            StringBuilder result = new StringBuilder("[");

            foreach (object current in enumerable)
            {
                if (result.Length > 1)
                {
                    result.Append(",");
                }

                result.Append(SerializeObject(current, dateTimeFormat));
            }

            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        /// Convert an IDictionary to a JSON string.
        /// </summary>
        /// <param name="dictionary">The value to convert.</param>
        /// <returns>The JSON object as a string or null when the value type is not supported.</returns>
        protected static string SerializeIDictionary(IDictionary dictionary, DateTimeFormat dateTimeFormat = DateTimeFormat.Default)
        {
            StringBuilder result = new StringBuilder("{");

            foreach (DictionaryEntry entry in dictionary)
            {
                if (result.Length > 1)
                {
                    result.Append(",");
                }

                result.Append("\"" + entry.Key + "\"");
                result.Append(":");
                result.Append(SerializeObject(entry.Value, dateTimeFormat));
            }

            result.Append("}");
            return result.ToString();
        }

        /// <summary>
        /// Safely serialize a String into a JSON string value, escaping all backslash and quote characters.
        /// </summary>
        /// <param name="str">The string to serialize.</param>
        /// <returns>The serialized JSON string.</returns>
        protected static string SerializeString(String str)
        {
            // If the string is just fine (most are) then make a quick exit for improved performance
            if (str.IndexOf('\\') < 0 && str.IndexOf('\"') < 0)
                return str;

            // Build a new string
            StringBuilder result = new StringBuilder(str.Length + 1); // we know there is at least 1 char to escape
            foreach (char ch in str.ToCharArray())
            {
                if (ch == '\\' || ch == '\"')
                    result.Append('\\');
                result.Append(ch);
            }
            return result.ToString();
        }
    }

    /// <summary>
    /// Enumeration of the popular formats of time and date
    /// within Json.  It's not a standard, so you have to
    /// know which on you're using.
    /// </summary>
    public enum DateTimeFormat
    {
        Default = 0,
        ISO8601 = 1,
        Ajax = 2
    }
}