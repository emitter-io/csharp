using System;

namespace Emitter.Network
{
    /// <summary>
    /// Represents emitter.io MQTT-based client.
    /// </summary>
    public class Emitter
    {
        #region Constructors
        /// <summary>
        /// The MQTT Client.
        /// </summary>
        private readonly MqttClient Client;

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

        public void Subscribe(string channel)
        {
            this.Client.Subscribe(new string[] { channel }, new byte[] { 0 });
        }

        public void Unsubscribe(string channel)
        {
            this.Client.Unsubscribe(new string[] { channel });
        }

        public void Publish(string channel, byte[] message)
        {
            this.Client.Publish(channel, message);
        }
    }
}