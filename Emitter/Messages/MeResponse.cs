using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Emitter.Messages
{
    public class MeResponse
    {
        public long? RequestId;

        public string MyId;

        private List<ShortcutInfo> Links;

        /// <summary>
        /// Deserializes the JSON response.
        /// </summary>
        /// <param name="json">The json string to deserialize from.</param>
        /// <returns></returns>
        public static MeResponse FromJson(string json)
        {
            var map = JsonSerializer.DeserializeString(json) as Hashtable;
            var response = new MeResponse();

            if (map.ContainsKey("req")) response.RequestId = (long)map["req"];
            response.MyId = (string)map["id"];
            response.Links = new List<ShortcutInfo>();

            /* TODO links
            if (map[""] is ArrayList)
            {
                foreach (Hashtable who in (ArrayList)map["who"])
                {
                    var info = new ShortcutInfo();
                    
                    info.Shortcut = (string)who["id"];
                    if (who.ContainsKey("username"))
                        info.Username = (string)who["username"];
                    response.Links.Add(info);
                }
            }*/

            return response;
        }

        /// <summary>
        /// Deserializes the JSON response.
        /// </summary>
        /// <param name="message">The binary UTF-8 encoded string.</param>
        /// <returns></returns>
        public static MeResponse FromBinary(byte[] message)
        {
            return FromJson(new string(Encoding.UTF8.GetChars(message)));
        }

        public class ShortcutInfo
        {
            public string Shortcut;
            public string Channel;
        }

    }
}
