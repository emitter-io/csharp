using System;
using System.Collections.Generic;
using System.Text;
using Emitter.Messages;

namespace Emitter
{
    public partial class Connection
    {
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
