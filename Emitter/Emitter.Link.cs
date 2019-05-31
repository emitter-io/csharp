using System;
using System.Collections.Generic;
using System.Text;
using Emitter.Messages;

namespace Emitter
{
    public partial class Connection
    {
        /// <summary>
        /// Create a short 2-character link for the channel. Uses the default key that should be specified in the constructor.
        /// </summary>
        /// <param name="channel">The channel to link to.</param>
        /// <param name="name">The name of the link to create.</param>
        /// <param name="isPrivate">Whether the link is private.</param>
        /// <param name="subscribe"></param>
        /// <param name="options"></param>
        public void Link(string channel, string name, bool isPrivate, bool subscribe, params string[] options)
        {
            if (this.DefaultKey == null)
                throw EmitterException.NoDefaultKey;

            this.Link(this.DefaultKey, channel, name, isPrivate, subscribe, options);
        }

        public void Link(string key, string channel, string name, bool isPrivate, bool subscribe, params string[] options)
        {
            var request = new LinkRequest();
            request.Key = key;
            request.Channel = this.FormatChannelLink(channel, options);
            request.Name = name;
            request.Private = isPrivate;
            request.Subscribe = subscribe;

            this.Publish("emitter/", "link", Encoding.UTF8.GetBytes(request.ToJson()));
        }

        // TODO options
        public ushort PublishWithLink(string name, byte[] message)
        {
            return this.Client.Publish(name, message);
        }

        public ushort PublishWithLink(string name, string message)
        {
            return this.Client.Publish(name, Encoding.UTF8.GetBytes(message));
        }
    }
}
