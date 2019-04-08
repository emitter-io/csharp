using System.Text;
using Emitter.Messages;

namespace Emitter
{
    public partial class Connection
    {
        #region Presence Members

        public event PresenceHandler Presence;

        public void PresenceSubscription(string key, string channel, bool status)
        {
            var request = new PresenceRequest();
            request.Key = key;
            request.Channel = channel;
            request.Status = status;
            request.Changes = true;

            this.Publish("emitter/", "presence", Encoding.UTF8.GetBytes(request.ToJson()));
        }

        #endregion Presence Members
    }
}