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

namespace Emitter.Messages
{
    /// <summary>
    /// Represents a key generation request.
    /// </summary>
    public class KeygenRequest
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
        /// Gets or sets the type of the requested key.
        /// </summary>
        public SecurityAccess Type;

        /// <summary>
        /// Gets or sets the number of seconds for which this key will be usable.
        /// </summary>
        public int Ttl;

        /// <summary>
        /// Converts the request to JSON format.
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            var keyType = "";
            if ((this.Type & SecurityAccess.Read) != 0)
                keyType += "r";
            if ((this.Type & SecurityAccess.Write) != 0)
                keyType += "w";
            if ((this.Type & SecurityAccess.Store) != 0)
                keyType += "s";
            if ((this.Type & SecurityAccess.Load) != 0)
                keyType += "l";
            if ((this.Type & SecurityAccess.Presence) != 0)
                keyType += "p";

            return JsonSerializer.SerializeObject(new Hashtable
            {
                {"key", this.Key},
                {"channel", this.Channel},
                {"type", keyType},
                {"ttl", this.Ttl}
            });
        }
    }

    /// <summary>
    /// Represents the key type.
    /// </summary>
    [Obsolete("Use SecurityAccess enum instead")]
    public enum EmitterKeyType : uint
    {
    }

    /// <summary>
    /// Represents the security access particular to a given key.
    /// </summary>
    [Flags]
    public enum SecurityAccess : uint
    {
        /// <summary>
        /// Key has no privileges.
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Key should be allowed to subscribe to the target channel.
        /// </summary>
        Read = 1 << 1,

        /// <summary>
        /// Key should be allowed to publish to the target channel.
        /// </summary>
        Write = 1 << 2,

        /// <summary>
        /// Key should be allowed to write to the message history of the target channel.
        /// </summary>
        Store = 1 << 3,

        /// <summary>
        /// Key should be allowed to read the message history of the target channel.
        /// </summary>
        Load = 1 << 4,

        /// <summary>
        /// Key should be allowed to query the presence on the target channel.
        /// </summary>
        Presence = 1 << 5,

        /// <summary>
        /// Key should be allowed to read and write to the target channel.
        /// </summary>
        ReadWrite = Read | Write,

        /// <summary>
        /// Key should be allowed to read and write the message history.
        /// </summary>
        StoreLoad = Store | Load

    }
}
