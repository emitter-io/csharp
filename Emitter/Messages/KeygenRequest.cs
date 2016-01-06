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

namespace Emitter.Network.Messages
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
        public EmitterKeyType Type;

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
            var keyType = "r";
            switch(this.Type)
            {
                case EmitterKeyType.WriteOnly: keyType = "w"; break;
                case EmitterKeyType.ReadWrite: keyType = "rw"; break;
                case EmitterKeyType.ReadOnly:
                default: keyType = "r"; break;
            }

            var map = new Hashtable();
            map.Add("key", this.Key);
            map.Add("channel", this.Channel);
            map.Add("type", keyType);
            map.Add("ttl", this.Ttl);

            return JsonSerializer.SerializeObject(map);
        }
    }

    /// <summary>
    /// Represents the key type.
    /// </summary>
    public enum EmitterKeyType
    {
        /// <summary>
        /// Requests the key to be a read-only key.
        /// </summary>
        ReadOnly  = 0,

        /// <summary>
        /// Requests the key to be a write-only key.
        /// </summary>
        WriteOnly = 1,

        /// <summary>
        /// Requests the key to be a read/write key.
        /// </summary>
        ReadWrite = 2
    }
}
