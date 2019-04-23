using System;
using System.Collections;

namespace Emitter.Utility
{
 /// <summary>
    /// Represents a trie with a reverse-pattern search.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReverseTrie<T> where T : class
    {
        // TODO
        public delegate T AddFunc();
        public delegate T UpdateFunc(T old);

        private readonly Hashtable Children;
        private readonly short Level = 0;
        private T Value = default(T);

        /// <summary>
        /// Constructs a node of the trie.
        /// </summary>
        /// <param name="level">The level of this node within the trie.</param>
        public ReverseTrie(short level)
        {
            this.Level = level;
            this.Children = new Hashtable();
        }

        /// <summary>
        /// Adds a new handler for the channel.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="value"></param>
        public void RegisterHandler(string channel, T value)
        {
            // Add the value or replace it.
            this.AddOrUpdate(CreateKey(channel), 0, () => value, (old) => value);
        }

        /// <summary>
        /// Unregister the handler from the trie.
        /// </summary>
        /// <param name="channel"></param>
        public void UnregisterHandler(string channel)
        {
            T removed;
            this.TryRemove(CreateKey(channel), 0, out removed);
        }

        private ArrayList RecurMatch(string[] query, int posInQuery, Hashtable children)
        {
            var matches = new ArrayList();
            if (posInQuery == query.Length)
                return matches;
            if (Utils.TryGetValueFromHashtable(children, "+", out object objPlusChildNode))
            {
                var childNode = objPlusChildNode as ReverseTrie<T>;
                if (childNode.Value != default(T))
                    matches.Add(childNode.Value);
                matches.AddRange(RecurMatch(query, posInQuery + 1, childNode.Children));
            }
            if (Utils.TryGetValueFromHashtable(children, query[posInQuery], out object objQueryChildNode))
            {
                var childNode = objQueryChildNode as ReverseTrie<T>;
                if (childNode.Value != default(T))
                    matches.Add(childNode.Value);
                matches.AddRange(RecurMatch(query, posInQuery + 1, childNode.Children));
            }
            return matches;
        }
        /// <summary>
        /// Retrieves a set of values.
        /// </summary>
        /// <returns></returns>
        public ArrayList Match(string channel)
        {
            var query = CreateKey(channel);
            var result = RecurMatch(query, 0, this.Children);
            return result;
        }

        /// <summary>
        /// Creates a query for the trie from the channel name.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static string[] CreateKey(string channel)
        {
            return channel.Trim('/').Split('/');
        }

        /// <summary>
        /// Adds or updates a specific value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private object AddOrUpdate(string[] key, int position, AddFunc addFunc, UpdateFunc updateFunc)
        {
            if (position == key.Length)
            {
                // TODO check lock's position
                lock (this)
                {
                    // There's already a value
                    if (this.Value != default(T))
                        return updateFunc(this.Value);

                    // No value, add it
                    this.Value = addFunc();
                    return this.Value;
                }
            }

            // Create a child
            var child = Utils.GetOrAddToHashtable(Children, key[position], new ReverseTrie<T>((short)position)) as ReverseTrie<T>;
            return child.AddOrUpdate(key, position + 1, addFunc, updateFunc);
        }

        /// <summary>
        /// Attempts to remove a specific key from the Trie.
        /// </summary>
        private bool TryRemove(string[] key, int position, out T value)
        {
            if (position == key.Length)
            {
                lock (this)
                {
                    // There's no value
                    value = this.Value;
                    if (this.Value == default(T))
                        return false;

                    this.Value = default(T);
                    return true;
                }
            }

            // Remove from the child
            object child;
            if (Utils.TryGetValueFromHashtable(Children, key[position], out child))
                return ((ReverseTrie<T>)child).TryRemove(key, position + 1, out value);

            value = default(T);
            return false;
        }
    }
}
