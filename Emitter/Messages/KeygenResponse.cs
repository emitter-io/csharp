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
    /// Represents a key generation response.
    /// </summary>
    public class KeygenResponse
    {
        public long? RequestId;

        /// <summary>
        /// Gets or sets the generated key.
        /// </summary>
        public string Key;

        /// <summary>
        /// Gets or sets the target channel for the supplied key.
        /// </summary>
        public string Channel;

        /// <summary>
        /// Gets or sets the status code returned by emitter.io service.
        /// </summary>
        public int Status;

        /// <summary>
        /// Deserializes the JSON key-gen response.
        /// </summary>
        /// <param name="json">The json string to deserialize from.</param>
        /// <returns></returns>
        public static KeygenResponse FromJson(string json)
        {
            var map = JsonSerializer.DeserializeString(json) as Hashtable;
            // Check for error.
            ErrorEvent.ThrowIfError(map);

            var response = new KeygenResponse();

            if (map.ContainsKey("req")) response.RequestId = (long)map["req"];
            if (map.ContainsKey("key")) response.Key = (string)map["key"];
            if (map.ContainsKey("channel")) response.Channel = (string)map["channel"];
            if (map.ContainsKey("status")) response.Status = Convert.ToInt32(map["status"].ToString());
            
            return response;
        }

        /// <summary>
        /// Deserializes the JSON key-gen response.
        /// </summary>
        /// <param name="message">The binary UTF-8 encoded string.</param>
        /// <returns></returns>
        public static KeygenResponse FromBinary(byte[] message)
        {
            return FromJson(new string(Encoding.UTF8.GetChars(message)));
        }

    }

    /// <summary>
    /// Handles the key generation response.
    /// </summary>
    /// <param name="response">The keygen response to handle.</param>
    public delegate void KeygenHandler(KeygenResponse response);
}