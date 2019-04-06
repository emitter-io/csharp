using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Emitter.Messages
{
    public class PresenceResponse
    {
        public int Request;

        public PresenceEvent Event;

        public string Channel;

        public int Time;

        public List<PresenceInfo> Who;
        
        /// <summary>
        /// Deserializes the JSON key-gen response.
        /// </summary>
        /// <param name="json">The json string to deserialize from.</param>
        /// <returns></returns>
        public static PresenceResponse FromJson(string json)
        {
            var map = JsonSerializer.DeserializeString(json) as Hashtable;
            var response = new PresenceResponse();

            response.Request = (int)map["req"];
            response.Channel = (string)map["channel"];
            response.Time = (int)map["time"];

            switch ((string)map["event"])
            {
                case "subscribe":
                    response.Event = PresenceEvent.Subscribe;
                    break;
                case "unsubscribe":
                    response.Event = PresenceEvent.Unsubscribe;
                    break;
            }

            response.Who = new List<PresenceInfo>();
            foreach (string infoString in (string[])map["who"])
                response.Who.Add(PresenceInfo.FromJson(infoString));

            return response;
        }

        /// <summary>
        /// Deserializes the JSON key-gen response.
        /// </summary>
        /// <param name="message">The binary UTF-8 encoded string.</param>
        /// <returns></returns>
        public static PresenceResponse FromBinary(byte[] message)
        {
            return FromJson(new string(Encoding.UTF8.GetChars(message)));
        }

        public enum PresenceEvent
        {
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
