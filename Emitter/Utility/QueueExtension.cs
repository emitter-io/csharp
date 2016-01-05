/*
Copyright (c) 2013, 2014 Paolo Patierno

All rights reserved. This program and the accompanying materials
are made available under the terms of the Eclipse Public License v1.0
and Eclipse Distribution License v1.0 which accompany this distribution. 

The Eclipse Public License is available at 
   http://www.eclipse.org/legal/epl-v10.html
and the Eclipse Distribution License is available at 
   http://www.eclipse.org/org/documents/edl-v10.php.

Contributors:
   Paolo Patierno - initial API and implementation and/or initial documentation
*/

using System;
using System.Collections;
using System.Threading;

namespace Emitter.Network.Utility
{
    /// <summary>
    /// Extension class for a Queue
    /// </summary>
    internal static class QueueExtension
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
        internal static object Get(this Queue queue, QueuePredicate predicate)
        {
            foreach (var item in queue)
            {
                if (predicate(item))
                    return item;
            }
            return null;
        }

#if MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3 || COMPACT_FRAMEWORK
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
#endif

        /// <summary>
        /// Attempts to fetch a value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool TryGetValue(this Hashtable source, object key, out object value)
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
            catch(Exception ex)
            {
#if TRACE
                Trace.WriteLine(TraceLevel.Error, ex.Message);
#endif
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
        internal static object GetOrAdd(this Hashtable source, object key, object value)
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
    }

    public delegate MessageHandler AddFunc();
    public delegate MessageHandler UpdateFunc(MessageHandler old);

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
        public override string ToString() { return "(" + Key + ", " + Value + ")"; }
    }

}

