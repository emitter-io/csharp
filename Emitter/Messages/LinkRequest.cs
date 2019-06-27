using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Emitter.Messages
{
    public class LinkRequest
    {
        /// <summary>
        /// Gets or sets the secret key for this request.
        /// </summary>
        public string Key;

        /// <summary>
        /// Gets or sets the target channel for the requested key.
        /// </summary>
        public string Channel;

        /// <summary>
        /// Gets or sets the name of the link.
        /// </summary>
        public string Name;

        /// <summary>
        /// Gets or sets whether the link is private.
        /// </summary>
        public bool Private;

        /// <summary>
        /// Gets or sets whether to subscribe automatically upon the creation of the link.
        /// </summary>
        public bool Subscribe;

        /// <summary>
        /// Converts the request to JSON format.
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonSerializer.SerializeObject(new Hashtable
            {
                {"key", this.Key},
                {"channel", this.Channel},
                {"name", this.Name},
                {"private", this.Private},
                {"subscribe", this.Subscribe }
            });
        }
    }
}
