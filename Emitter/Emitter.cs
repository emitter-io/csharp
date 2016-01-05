using System;
using System.Collections;
using Emitter.Network.Utility;
#if (MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3)
using Microsoft.SPOT;
#endif


namespace Emitter.Network
{
    /// <summary>
    /// Represents emitter.io MQTT-based client.
    /// </summary>
    public class Emitter
    {
        #region Constructors
        private readonly MqttClient Client;
        private readonly ReverseTrie Trie = new ReverseTrie(-1);

        /// <summary>
        /// Constructs a new emitter.io connection.
        /// </summary>
        public Emitter() : this("api.emitter.io") { }

        /// <summary>
        /// Constructs a new emitter.io connection.
        /// </summary>
        /// <param name="broker">The broker hostname to use.</param>
        public Emitter(string broker)
        {
            this.Client = new MqttClient(broker);
            this.Client.MqttMsgPublishReceived += OnMessageReceived;
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessageReceived(object sender, Messages.MqttMsgPublishEventArgs e)
        {
            // Invoke every handler matching the channel
            foreach(MessageHandler handler in this.Trie.Match(e.Topic))
                handler(e.Topic, e.Message);
        }
        #endregion

        public void Connect()
        {
            var connack = this.Client.Connect(Guid.NewGuid().ToString());
        }

        public void Disconnect()
        {
            this.Client.Disconnect();
        }

        public void On(string key, string channel, MessageHandler handler)
        {
            // Register the handler
            this.Trie.RegisterHandler(channel, handler);

            // Subscribe
            this.Client.Subscribe(new string[] { channel }, new byte[] { 0 });
        }

        public void Unsubscribe(string key, string channel)
        {
            // Unregister the handler
            this.Trie.UnregisterHandler(key);

            // Unsubscribe
            this.Client.Unsubscribe(new string[] { channel });
        }

        public void Publish(string key, string channel, byte[] message)
        {
            this.Client.Publish(channel, message);
        }
    }

    #region Delegates
    /// <summary>
    /// Represents a message handler callback.
    /// </summary>
    public delegate void MessageHandler(string channel, byte[] message);

    #endregion

    #region ReverseTrie
    /// <summary>
    /// Represents a trie with a reverse-pattern search.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ReverseTrie
    {
        private readonly Hashtable Children;
        private readonly short Level = 0;
        private MessageHandler Value = default(MessageHandler);

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
        public void RegisterHandler(string channel, MessageHandler value)
        {
            // Add the value or replace it.
            this.AddOrUpdate(this.CreateKey(channel), 0, () => value, (old) => value);
        }

        /// <summary>
        /// Unregister the handler from the trie.
        /// </summary>
        /// <param name="channel"></param>
        public void UnregisterHandler(string channel)
        {
            MessageHandler removed;
            this.TryRemove(this.CreateKey(channel), 0, out removed);
        }
        
        /// <summary>
        /// Retrieves a set of values.
        /// </summary>
        /// <param name="query">The query to retrieve.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public IEnumerable Match(string channel)
        {
            // Get the query
            var query = this.CreateKey(channel);

            // Get the matching stack
            var matches = new Stack();

            // Push the root
            object childNode;
            matches.Push(this);
            while (matches.Count != 0)
            {
                var current = matches.Pop() as ReverseTrie;
                if (current.Value != default(object))
                    yield return current.Value;

                var level = current.Level + 1;
                if (level >= query.Length)
                    break;

                if (current.Children.TryGetValue("+", out childNode))
                    matches.Push(childNode);
                if (current.Children.TryGetValue(query[level], out childNode))
                    matches.Push(childNode);
            }
        }

        #region Private Members
        /// <summary>
        /// Creates a query for the trie from the channel name.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        private string[] CreateKey(string channel)
        {
            return channel.Split('/');
        }

        /// <summary>
        /// Adds or updates a specific value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private object AddOrUpdate(string[] key, int position, AddFunc addFunc, UpdateFunc updateFunc)
        {
            if (position >= key.Length)
            {
                lock (this)
                {
                    // There's already a value
                    if (this.Value != default(object))
                        return updateFunc(this.Value);

                    // No value, add it
                    this.Value = addFunc();
                    return this.Value;
                }
            }

            // Create a child
            var child = Children.GetOrAdd(key[position], new ReverseTrie((short)position)) as ReverseTrie;
            return child.AddOrUpdate(key, position + 1, addFunc, updateFunc);
        }


        /// <summary>
        /// Attempts to remove a specific key from the Trie.
        /// </summary>
        private bool TryRemove(string[] key, int position, out MessageHandler value)
        {
            if (position >= key.Length)
            {
                lock (this)
                {
                    // There's no value
                    value = this.Value;
                    if (this.Value == default(MessageHandler))
                        return false;

                    this.Value = default(MessageHandler);
                    return true;
                }
            }

            // Remove from the child
            object child;
            if (Children.TryGetValue(key[position], out child))
                return ((ReverseTrie)child).TryRemove(key, position + 1, out value);

            value = default(MessageHandler);
            return false;
        }
        #endregion
    }
    #endregion
}