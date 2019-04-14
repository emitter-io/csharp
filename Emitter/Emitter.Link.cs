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

            this.Link(this.DefaultKey, channel, isPrivate, subscribe, options);
        }

        public void Link(string key, string channel, string name, bool isPrivate, bool subscribe, params string[] options)
        {
            var request = new LinkRequest();
            request.Key = key;
            request.Channel = this.FormatChannel(null, channel, options);
            request.Name = name;
            request.Private = isPrivate;
            request.Subscribe = subscribe;

            this.Publish("emitter/", "presence", Encoding.UTF8.GetBytes(request.ToJson()));
        }
    }
}
