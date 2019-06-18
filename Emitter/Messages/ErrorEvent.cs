using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Emitter.Messages
{
    public class ErrorEvent
    {
        public long? RequestId;
        public long Status;
        public string Message;

        public static ErrorEvent FromJson(string json)
        {
            var map = JsonSerializer.DeserializeString(json) as Hashtable;
            var error = new ErrorEvent();

            if (map.ContainsKey("req")) error.RequestId = (long)map["req"];
            error.Status = (long)map["status"];
            error.Message = (string)map["message"];

            return error;
        }

        public static ErrorEvent FromBinary(byte[] message)
        {
            return FromJson(new string(Encoding.UTF8.GetChars(message)));
        }

        public static void ThrowIfError(Hashtable map)
        {
            if (map.ContainsKey("status") && (long)map["status"] != 200)
            {
                throw new EmitterException((EmitterEventCode)map["status"], (string)map["message"]);
            }
        }
    }
}
