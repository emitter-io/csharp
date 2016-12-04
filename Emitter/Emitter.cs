/*
Copyright (c) 2016 Roman Atachiants

All rights reserved. This program and the accompanying materials
are made available under the terms of the Eclipse Public License v1.0
and Eclipse Distribution License v1.0 which accompany this distribution.

The Eclipse Public License:  http://www.eclipse.org/legal/epl-v10.html
The Eclipse Distribution License: http://www.eclipse.org/org/documents/edl-v10.php.

Contributors:
   Roman Atachiants - integrating with emitter.io
*/

using System;
using System.Collections;
using System.Text;

using Emitter.Messages;

#if (MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3)

using Microsoft.SPOT;

#endif

namespace Emitter
{
    /// <summary>
    /// Represents a message handler callback.
    /// </summary>
    public delegate void MessageHandler(string channel, byte[] message);

    /// <summary>
    /// Delegate that defines event handler for cliet/peer disconnection
    /// </summary>
    public delegate void DisconnectHandler(object sender, EventArgs e);

    /// <summary>
    /// Delegate that defines an event handler for an error.
    /// </summary>
    public delegate void ErrorHandler(object sender, Exception e);

    /// <summary>
    /// Represents emitter.io MQTT-based client.
    /// </summary>
    public class Connection : IDisposable
    {
        #region Constructors
        private readonly MqttClient Client;
        private readonly ReverseTrie Trie = new ReverseTrie(-1);
        private string DefaultKey = null;

        /// <summary>
        /// Constructs a new emitter.io connection.
        /// </summary>
        public Connection() : this(null, 0, null) { }

        /// <summary>
        /// Constructs a new emitter.io connection.
        /// </summary>
        /// <param name="defaultKey">The default key to use.</param>
        public Connection(string defaultKey) : this(null, 0, defaultKey) { }

        /// <summary>
        /// Constructs a new emitter.io connection.
        /// </summary>
        /// <param name="broker">The broker hostname to use.</param>
        /// <param name="defaultKey">The default key to use.</param>
        /// <param name="useTls">Whether we should use TLS security.</param>
        public Connection(string broker, int brokerPort, string defaultKey)
        {
            if (broker == null)
                broker = "api.emitter.io";
            if (brokerPort <= 0)
                brokerPort = MqttSettings.MQTT_BROKER_DEFAULT_PORT;

            this.DefaultKey = defaultKey;
            this.Client = new MqttClient(broker, brokerPort);
            this.Client.MqttMsgPublishReceived += OnMessageReceived;
            this.Client.ConnectionClosed += OnDisconnect;
        }

        #endregion Constructors

        #region Error Members

        /// <summary>
        /// Occurs when an error occurs.
        /// </summary>
        public event ErrorHandler Error;

        /// <summary>
        /// Invokes the error handler.
        /// </summary>
        /// <param name="ex"></param>
        private void InvokeError(Exception ex)
        {
            if (this.Error != null)
                this.Error(this, ex);
        }

        /// <summary>
        /// Invokes the error handler.
        /// </summary>
        /// <param name="status"></param>
        private void InvokeError(int status)
        {
            if (status == 200)
                return;

            InvokeError(EmitterException.FromStatus(status));
        }

        #endregion Error Members

        #region Static Members

        /// <summary>
        /// Gets the default instance of the client.
        /// </summary>
        public static readonly Connection Default = new Connection();

        /// <summary>
        /// Establishes a new connection by creating the connection instance and connecting to it.
        /// </summary>
        /// <returns>The connection state.</returns>
        public static Connection Establish()
        {
            return Establish(null, 0, null);
        }

        /// <summary>
        /// Establishes a new connection by creating the connection instance and connecting to it.
        /// </summary>
        /// <param name="defaultKey">The default key to use.</param>
        /// <returns>The connection state.</returns>
        public static Connection Establish(string defaultKey)
        {
            return Establish(null, 0, defaultKey);
        }

        /// <summary>
        /// Establishes a new connection by creating the connection instance and connecting to it.
        /// </summary>
        /// <param name="brokerHostName">The broker hostname to use.</param>
        /// <param name="defaultKey">The default key to use.</param>
        /// <param name="useTls">Whether we should use TLS security.</param>
        /// <returns>The connection state.</returns>
        public static Connection Establish(string brokerHostName, int brokerPort, string defaultKey)
        {
            // Create the connection
            var conn = new Connection(brokerHostName, brokerPort, defaultKey);

            // Connect
            conn.Connect();

            // Return it
            return conn;
        }

        #endregion Static Members

        #region Connect / Disconnect Members

        /// <summary>
        /// Occurs when the client was disconnected.
        /// </summary>
        public event DisconnectHandler Disconnected;

        /// <summary>
        /// Gets whether the client is connected or not.
        /// </summary>
        public bool IsConnected
        {
            get { return this.Client.IsConnected; }
        }

        /// <summary>
        /// Occurs when the connection was closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisconnect(object sender, EventArgs e)
        {
            // Forward the event
            if (this.Disconnected != null)
                this.Disconnected(sender, e);
        }

        /// <summary>
        /// Connects the emitter.io service.
        /// </summary>
        public void Connect()
        {
            var connack = this.Client.Connect(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Disconnects from emitter.io service.
        /// </summary>
        public void Disconnect()
        {
            this.Client.Disconnect();
        }

        #endregion Connect / Disconnect Members

        #region Subscribe / Unsubscribe Members

        /// <summary>
        /// Asynchronously subscribes to a particular channel of emitter.io service. Uses the default
        /// key that should be specified in the constructor.
        /// </summary>
        /// <param name="channel">The channel to subscribe to.</param>
        /// <param name="handler">The callback to be invoked every time the message is received.</param>
        /// <returns>The message identifier for this operation.</returns>
        public ushort On(string channel, MessageHandler handler)
        {
            if (this.DefaultKey == null)
                throw EmitterException.NoDefaultKey;
            return this.On(this.DefaultKey, channel, handler);
        }

        /// <summary>
        /// Asynchronously subscribes to a particular channel of emitter.io service.
        /// </summary>
        /// <param name="key">The key to use for this subscription request.</param>
        /// <param name="channel">The channel to subscribe to.</param>
        /// <param name="handler">The callback to be invoked every time the message is received.</param>
        /// <returns>The message identifier for this operation.</returns>
        public ushort On(string key, string channel, MessageHandler handler)
        {
            // Register the handler
            this.Trie.RegisterHandler(channel, handler);

            // Subscribe
            return this.Client.Subscribe(new string[] { FormatChannel(key, channel) }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        }

        /// <summary>
        /// Asynchronously subscribes to a particular channel of emitter.io service.
        /// </summary>
        /// <param name="key">The key to use for this subscription request.</param>
        /// <param name="channel">The channel to subscribe to.</param>
        /// <param name="handler">The callback to be invoked every time the message is received.</param>
        /// <param name="last">The last x messages to request when we subcribe.</param>
        /// <returns>The message identifier for this operation.</returns>
        public ushort On(string key, string channel, MessageHandler handler, int last)
        {
            // Register the handler
            this.Trie.RegisterHandler(channel, handler);

            // Subscribe
            return this.Client.Subscribe(new string[] { FormatChannel(key, channel, new Option("last", last.ToString())) }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        }

        /// <summary>
        /// Asynchonously unsubscribes from a particular channel of emitter.io service. Uses the default
        /// key that should be specified in the constructor.
        /// </summary>
        /// <param name="channel">The channel to subscribe to.</param>
        /// <returns>The message identifier for this operation.</returns>
        public ushort Unsubscribe(string channel)
        {
            if (this.DefaultKey == null)
                throw EmitterException.NoDefaultKey;
            return this.Unsubscribe(this.DefaultKey, channel);
        }

        /// <summary>
        /// Asynchonously unsubscribes from a particular channel of emitter.io service.
        /// </summary>
        /// <param name="key">The key to use for this unsubscription request.</param>
        /// <param name="channel">The channel to subscribe to.</param>
        /// <returns>The message identifier for this operation.</returns>
        public ushort Unsubscribe(string key, string channel)
        {
            // Unregister the handler
            this.Trie.UnregisterHandler(key);

            // Unsubscribe
            return this.Client.Unsubscribe(new string[] { FormatChannel(key, channel) });
        }

        #endregion Subscribe / Unsubscribe Members

        #region Publish Members

        /// <summary>
        /// Asynchonously publishes a message to the emitter.io service. Uses the default
        /// key that should be specified in the constructor.
        /// </summary>
        /// <param name="channel">The channel to publish to.</param>
        /// <param name="message">The message body to send.</param>
        /// <returns>The message identifier for this operation.</returns>
        public ushort Publish(string channel, byte[] message)
        {
            if (this.DefaultKey == null)
                throw EmitterException.NoDefaultKey;
            return this.Publish(this.DefaultKey, channel, message);
        }

        /// <summary>
        /// Asynchonously publishes a message to the emitter.io service. Uses the default
        /// key that should be specified in the constructor.
        /// </summary>
        /// <param name="channel">The channel to publish to.</param>
        /// <param name="message">The message body to send.</param>
        /// <returns>The message identifier for this operation.</returns>
        public ushort Publish(string channel, string message)
        {
            if (this.DefaultKey == null)
                throw EmitterException.NoDefaultKey;
            return this.Publish(this.DefaultKey, channel, Encoding.UTF8.GetBytes(message));
        }

        /// <summary>
        /// Publishes a message to the emitter.io service asynchronously.
        /// </summary>
        /// <param name="key">The key to use for this publish request.</param>
        /// <param name="channel">The channel to publish to.</param>
        /// <param name="message">The message body to send.</param>
        /// <returns>The message identifier.</returns>
        public ushort Publish(string key, string channel, string message)
        {
            return this.Client.Publish(FormatChannel(key, channel), Encoding.UTF8.GetBytes(message));
        }

        /// <summary>
        /// Publishes a message to the emitter.io service asynchronously.
        /// </summary>
        /// <param name="key">The key to use for this publish request.</param>
        /// <param name="channel">The channel to publish to.</param>
        /// <param name="message">The message body to send.</param>
        /// <returns>The message identifier.</returns>
        public ushort Publish(string key, string channel, byte[] message)
        {
            return this.Client.Publish(FormatChannel(key, channel), message);
        }

        /// <summary>
        /// Publishes a message to the emitter.io service asynchronously.
        /// </summary>
        /// <param name="key">The key to use for this publish request.</param>
        /// <param name="channel">The channel to publish to.</param>
        /// <param name="message">The message body to send.</param>
        /// <param name="ttl">The time to live of the message.</param>
        /// <returns>The message identifier.</returns>
        public ushort Publish(string key, string channel, string message, int ttl)
        {
            return this.Client.Publish(FormatChannel(key, channel, new Option("ttl", ttl.ToString())), Encoding.UTF8.GetBytes(message));
        }

        /// <summary>
        /// Publishes a message to the emitter.io service asynchronously.
        /// </summary>
        /// <param name="key">The key to use for this publish request.</param>
        /// <param name="channel">The channel to publish to.</param>
        /// <param name="message">The message body to send.</param>
        /// <param name="ttl">The time to live of the message.</param>
        /// <returns>The message identifier.</returns>
        public ushort Publish(string key, string channel, byte[] message, int ttl)
        {
            return this.Client.Publish(FormatChannel(key, channel, new Option("ttl", ttl.ToString())), message);
        }

        #endregion Publish Members

        #region KeyGen Members

        /// <summary>
        /// Hashtable used for processing keygen responses.
        /// </summary>
        private readonly Hashtable KeygenHandlers = new Hashtable();

        /// <summary>
        /// Asynchronously sends a key generation request to the emitter.io service.
        /// </summary>
        /// <param name="secretKey">The secret key for this request.</param>
        /// <param name="channel">The target channel for the requested key.</param>
        /// <param name="keyType">The type of the requested key.</param>
        public void GenerateKey(string secretKey, string channel, EmitterKeyType keyType, KeygenHandler handler)
        {
            this.GenerateKey(secretKey, channel, keyType, 0, handler);
        }

        /// <summary>
        /// Asynchronously sends a key generation request to the emitter.io service.
        /// </summary>
        /// <param name="secretKey">The secret key for this request.</param>
        /// <param name="channel">The target channel for the requested key.</param>
        /// <param name="keyType">The type of the requested key.</param>
        /// <param name="ttl">The number of seconds for which this key will be usable.</param>
        public void GenerateKey(string secretKey, string channel, EmitterKeyType keyType, int ttl, KeygenHandler handler)
        {
            // Prepare the request
            var request = new KeygenRequest();
            request.Key = secretKey;
            request.Channel = channel;
            request.Type = keyType;
            request.Ttl = ttl;

            // Register the handler
            this.KeygenHandlers[channel] = handler;

            //this.Client.Subscribe(new string[] { "emitter/keygen/" }, new byte[] { 0 });

            // Serialize and publish the request
            this.Publish("emitter/", "keygen/", Encoding.UTF8.GetBytes(request.ToJson()));
        }

        #endregion KeyGen Members

        #region Private Members

        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessageReceived(object sender, Messages.MqttMsgPublishEventArgs e)
        {
            try
            {
                // Did we receive a keygen response?
                if (e.Topic == "emitter/keygen/")
                {
                    // Deserialize the response
                    var response = KeygenResponse.FromBinary(e.Message);
                    if (response == null || response.Status != 200)
                    {
                        this.InvokeError(response.Status);
                        return;
                    }

                    // Call the response handler we have registered previously
                    if (this.KeygenHandlers.ContainsKey(response.Channel))
                        ((KeygenHandler)this.KeygenHandlers[response.Channel])(response);
                    return;
                }
            }
            catch (Exception ex)
            {
                this.InvokeError(ex);
            }

            // Invoke every handler matching the channel
            foreach (MessageHandler handler in this.Trie.Match(e.Topic))
                handler(e.Topic, e.Message);
        }

        /// <summary>
        /// Formats the channel.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="channel">The channel name.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        private string FormatChannel(string key, string channel, params Option[] options)
        {
            // Prefix with the key
            var formatted = key.EndsWith("/")
                ? key + channel
                : key + "/" + channel;

            // Add trailing slash
            if (!formatted.EndsWith("/"))
                formatted += "/";

            // Add options
            if (options != null && options.Length > 0)
            {
                formatted += "?";
                for (int i = 0; i < options.Length; ++i)
                {
                    formatted += options[i].Key + "=" + options[i].Value;
                    if (i + 1 < options.Length)
                        formatted += "&";
                }
            }

            // We're done compiling the channel name
            return formatted;
        }

        #endregion Private Members

        #region IDisposable

        /// <summary>
        /// Disposes the connection.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the connection.
        /// </summary>
        /// <param name="disposing">Whether we are disposing or finalizing.</param>
        protected void Dispose(bool disposing)
        {
            try
            {
                this.Disconnect();
            }
            catch { }
        }

        /// <summary>
        /// Finalizes the connection.
        /// </summary>
        ~Connection()
        {
            this.Dispose(false);
        }

        #endregion IDisposable
    }

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
            this.AddOrUpdate(CreateKey(channel), 0, () => value, (old) => value);
        }

        /// <summary>
        /// Unregister the handler from the trie.
        /// </summary>
        /// <param name="channel"></param>
        public void UnregisterHandler(string channel)
        {
            MessageHandler removed;
            this.TryRemove(CreateKey(channel), 0, out removed);
        }

        /// <summary>
        /// Retrieves a set of values.
        /// </summary>
        /// <param name="query">The query to retrieve.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public IEnumerable Match(string channel)
        {
            // Matches
            var result = new ArrayList();

            // Get the query
            var query = CreateKey(channel);

            // Get the matching stack
            var matches = new Stack();

            // Push the root
            object childNode;
            matches.Push(this);
            while (matches.Count != 0)
            {
                var current = matches.Pop() as ReverseTrie;
                if (current.Value != default(object))
                    result.Add(current.Value);

                var level = current.Level + 1;
                if (level >= query.Length)
                    break;

                if (Utils.TryGetValueFromHashtable(current.Children, "+", out childNode))
                    matches.Push(childNode);
                if (Utils.TryGetValueFromHashtable(current.Children, query[level], out childNode))
                    matches.Push(childNode);
            }

            return result;
        }

        #region Private Members

        /// <summary>
        /// Creates a query for the trie from the channel name.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static string[] CreateKey(string channel)
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
            var child = Utils.GetOrAddToHashtable(Children, key[position], new ReverseTrie((short)position)) as ReverseTrie;
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
            if (Utils.TryGetValueFromHashtable(Children, key[position], out child))
                return ((ReverseTrie)child).TryRemove(key, position + 1, out value);

            value = default(MessageHandler);
            return false;
        }

        #endregion Private Members
    }

    #endregion ReverseTrie
}