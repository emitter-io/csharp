using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Emitter.Messages
{
    public class PresenceEvent
    {
        public int Request;

        public PresenceEventType Event;

        public string Channel;

        public long Time;

        public List<PresenceInfo> Who;
        
        /// <summary>
        /// Deserializes the JSON key-gen response.
        /// </summary>
        /// <param name="json">The json string to deserialize from.</param>
        /// <returns></returns>
        public static PresenceEvent FromJson(string json)
        {
            var map = JsonSerializer.DeserializeString(json) as Hashtable;
            var response = new Emitter.Messages.PresenceEvent();

            //response.Request = (int)map["req"];
            response.Channel = (string)map["channel"];
            response.Time = (long)map["time"];

            switch ((string)map["event"])
            {
                case "status":
                    response.Event = PresenceEventType.Status;
                    break;
                case "subscribe":
                    response.Event = PresenceEventType.Subscribe;
                    break;
                case "unsubscribe":
                    response.Event = PresenceEventType.Unsubscribe;
                    break;
            }

            response.Who = new List<PresenceInfo>();
            if (map["who"] is ArrayList)
            {
                foreach (Hashtable who in (ArrayList)map["who"])
                {
                    var info = new PresenceInfo();
                    info.Id = (string)who["id"];
                    if (who.ContainsKey("username"))
                        info.Username = (string)who["username"];
                    response.Who.Add(info);
                }
            }
            else if (map["who"] is Hashtable)
            {
                var who = (Hashtable)map["who"];
                var info = new PresenceInfo();
                info.Id = (string)who["id"];
                if (who.ContainsKey("username"))
                    info.Username = (string)who["username"];
                response.Who.Add(info);
            }
            return response;
        }

        /// <summary>
        /// Deserializes the JSON key-gen response.
        /// </summary>
        /// <param name="message">The binary UTF-8 encoded string.</param>
        /// <returns></returns>
        public static PresenceEvent FromBinary(byte[] message)
        {
            return FromJson(new string(Encoding.UTF8.GetChars(message)));
        }

        public enum PresenceEventType
        {
            Unknown,
            Status,
            Subscribe,
            Unsubscribe
        };

        public class PresenceInfo
        {
            public string Id;
            public string Username;

            public static PresenceInfo FromJson(string json)
            {
                var map = JsonSerializer.DeserializeString(json) as Hashtable;
                var info = new PresenceInfo();

                info.Id = (string)map["id"];
                info.Username = (string)map["username"];

                return info;
            }
        }
    }
}
