using System.Text;
using Emitter.Messages;
using Emitter.Utility;

namespace Emitter
{
    public partial class Connection
    {
        #region Presence Members

        /// <summary>
        /// Represents a Presence handler callback.
        /// </summary>
        public delegate void PresenceHandler(PresenceEvent presenceResponse);

        private readonly ReverseTrie<PresenceHandler> PresenceTrie = new ReverseTrie<PresenceHandler>(-1);

        public void PresenceSubscribe(string channel, bool status, PresenceHandler handler)
        {
            if (this.DefaultKey == null)
                throw EmitterException.NoDefaultKey;

            this.PresenceSubscribe(this.DefaultKey, channel, status, handler);
        }

        public void PresenceSubscribe(string key, string channel, bool status, PresenceHandler handler)
        {
            this.PresenceTrie.RegisterHandler(channel, handler);

            var request = new PresenceRequest();
            request.Key = key;
            request.Channel = channel;
            request.Status = status;
            request.Changes = true;

            this.Publish("emitter/", "presence", Encoding.UTF8.GetBytes(request.ToJson()));
        }

        public void PresenceUnsubscribe(string channel)
        {
            if (this.DefaultKey == null)
                throw EmitterException.NoDefaultKey;

            this.PresenceUnsubscribe(this.DefaultKey, channel);
        }

        public void PresenceUnsubscribe(string key, string channel)
        {
            this.PresenceTrie.UnregisterHandler(channel);

            var request = new PresenceRequest();
            request.Key = key;
            request.Channel = channel;
            request.Status = false;
            request.Changes = false;

            this.Publish("emitter/", "presence", Encoding.UTF8.GetBytes(request.ToJson()));
        }
        
        public void PresenceStatus(string channel, PresenceHandler handler)
        {
            if (this.DefaultKey == null)
                throw EmitterException.NoDefaultKey;

            this.PresenceStatus(this.DefaultKey, channel, handler);
        }

        public void PresenceStatus(string key, string channel, PresenceHandler handler)
        {
            this.PresenceTrie.RegisterHandler(channel, handler);

            var request = new PresenceRequest();
            request.Key = key;
            request.Channel = channel;
            request.Status = true;
            request.Changes = null;

            this.Publish("emitter/", "presence", Encoding.UTF8.GetBytes(request.ToJson()));
        }
        
        #endregion Presence Members
    }
}