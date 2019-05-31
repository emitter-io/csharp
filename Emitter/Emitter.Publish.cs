using System.Text;
using Emitter.Utility;

namespace Emitter
{
    public partial class Connection
    {
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
            return this.Client.Publish(FormatChannel(key, channel, Options.WithTTL(ttl)),
                Encoding.UTF8.GetBytes(message));
        }

        /// <summary>
        /// Publishes a message to the emitter.io service asynchronously.
        /// </summary>
        /// <param name="key">The key to use for this publish request.</param>
        /// <param name="channel">The channel to publish to.</param>
        /// <param name="message">The message body to send.</param>
        /// <param name="options">The options associated with the message. Ex: Options.WithLast(5).</param>
        /// <returns>The message identifier.</returns>
        public ushort Publish(string key, string channel, string message, params string[] options)
        {
            GetHeader(options, out var retain, out var qos);
            return this.Client.Publish(FormatChannel(key, channel, options), Encoding.UTF8.GetBytes(message), qos,
                retain);
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
            return this.Client.Publish(FormatChannel(key, channel, Options.WithTTL(ttl)), message);
        }

        /// <summary>
        /// Publishes a message to the emitter.io service asynchronously.
        /// </summary>
        /// <param name="key">The key to use for this publish request.</param>
        /// <param name="channel">The channel to publish to.</param>
        /// <param name="message">The message body to send.</param>
        /// <param name="options">The options associated with the message. Ex: Options.WithoutEcho().</param>
        /// <returns>The message identifier.</returns>
        public ushort Publish(string key, string channel, byte[] message, params string[] options)
        {
            GetHeader(options, out var retain, out var qos);
            return this.Client.Publish(FormatChannel(key, channel, options), message, qos, retain);
        }

        #endregion Publish Members
    }
}