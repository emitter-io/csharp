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
using Emitter.Utility;

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
    /// Represents a Presence handler callback.
    /// </summary>
    public delegate void PresenceHandler(PresenceEvent presenceResponse);

    /// <summary>
    /// Delegate that defines event handler for client/peer disconnection
    /// </summary>
    public delegate void DisconnectHandler(object sender, EventArgs e);

    /// <summary>
    /// Delegate that defines an event handler for an error.
    /// </summary>
    public delegate void ErrorHandler(object sender, Exception e);

    /// <summary>
    /// Represents emitter.io MQTT-based client.
    /// </summary>
    public partial class Connection : IDisposable
    {
        #region Constructors
        private readonly MqttClient Client;
        private readonly ReverseTrie Trie = new ReverseTrie(-1);
        private readonly ReverseTrie PresenceTrie = new ReverseTrie(-1);
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
            this.Error?.Invoke(this, ex);
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
            this.Disconnected?.Invoke(sender, e);
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

        //public event PresenceHandler Presence;


        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessageReceived(object sender, Messages.MqttMsgPublishEventArgs e)
        {
            try
            {
                if (!e.Topic.StartsWith("emitter"))
                {
                    // Invoke every handler matching the channel
                    foreach (MessageHandler handler in this.Trie.Match(e.Topic))
                        handler(e.Topic, e.Message);

                    return;
                }

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
                    // TODO: get rid of the handler afterwards, or refac keygen
                    if (this.KeygenHandlers.ContainsKey(response.Channel))
                        ((KeygenHandler)this.KeygenHandlers[response.Channel])(response);
                    return;
                }

                if (e.Topic == "emitter/presence/")
                {
                    var presenceEvent = PresenceEvent.FromBinary(e.Message);
                    Presence?.Invoke(presenceEvent);
                }
            }
            catch (Exception ex)
            {
                this.InvokeError(ex);
            }
        }
        #region Private Members
        /// <summary>
        /// Formats the channel.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="channel">The channel name.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        private string FormatChannel(string key, string channel, params string[] options)
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
                    if (options[i][0] == '+')
                        continue;

                    formatted += options[i];
                    if (i + 1 < options.Length)
                        formatted += "&";
                }
            }

            // We're done compiling the channel name
            return formatted;
        }

        private void GetHeader(string[] options, out bool retain, out byte qos)
        {
            retain = false;
            qos = 0;
            foreach (string o in options)
            {
                switch (o)
                {
                    case Options.Retain:
                        retain = true;
                        break;
                    case Options.QoS1:
                        qos = 1;
                        break;
                }
            }
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
}