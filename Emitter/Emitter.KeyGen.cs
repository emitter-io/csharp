using System.Text;
using Emitter.Messages;
using System.Collections;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace Emitter
{
    public partial class Connection
    {
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
        /// <param name="securityAccess">The security access of the requested key.</param>
        public void GenerateKey(string secretKey, string channel, SecurityAccess securityAccess, KeygenHandler handler)
        {
            this.GenerateKey(secretKey, channel, securityAccess, 0, handler);
        }

        /// <summary>
        /// Asynchronously sends a key generation request to the emitter.io service.
        /// </summary>
        /// <param name="secretKey">The secret key for this request.</param>
        /// <param name="channel">The target channel for the requested key.</param>
        /// <param name="securityAccess">The security access of the requested key.</param>
        /// <param name="ttl">The number of seconds for which this key will be usable.</param>
        public void GenerateKey(string secretKey, string channel, SecurityAccess securityAccess, int ttl,
            KeygenHandler handler)
        {
            // Prepare the request
            var request = new KeygenRequest();
            request.Key = secretKey;
            request.Channel = channel;
            request.Type = securityAccess;
            request.Ttl = ttl;

            // Register the handler
            this.KeygenHandlers[channel] = handler;

            //this.Client.Subscribe(new string[] { "emitter/keygen/" }, new byte[] { 0 });

            // Serialize and publish the request
            this.Publish("emitter/", "keygen/", Encoding.UTF8.GetBytes(request.ToJson()));
        }

        #if FX40 != true
        /// <summary>
        /// Sends a key generation request to the emitter.io service and returns the generated key
        /// Generates a timeout exception if the request times out
        /// </summary>
        /// <param name="secretKey">The secret key for this request.</param>
        /// <param name="channel">The target channel for the requested key.</param>
        /// <param name="securityAccess">The security access of the requested key.</param>
        /// <param name="ttl">The number of seconds for which this key will be usable.</param>
        /// <param name="timeout">Timout value. (Default value is 5 seconds)</param>
        public async Task<string> KeyGen(string secretKey, string channel, SecurityAccess securityAccess, int ttl, TimeSpan? timeout = null)
        {
            // Prepare the request
            var request = new KeygenRequest();
            request.Key = secretKey;
            request.Channel = channel;
            request.Type = securityAccess;
            request.Ttl = ttl;
            if (timeout == null) timeout = _defaultTimeout;
            // Execute the request
            var response = await ExecuteAsync(timeout.Value, "emitter/keygen/", Encoding.UTF8.GetBytes(request.ToJson()), CancellationToken.None);
            if (response != null)
            {
                var map = JsonSerializer.DeserializeString(Encoding.UTF8.GetString(response)) as Hashtable;
                if (map.ContainsKey("key")) return map["key"].ToString();
            }
            return "";
        }
        #endif
        #endregion KeyGen Members
    }
}