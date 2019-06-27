using System;
using System.Collections.Generic;
using System.Text;

namespace Emitter.Utility
{
    public static class Options
    {
        public const string Retain = "+r";
        public const string QoS0 = "+0";
        public const string QoS1 = "+1";

        public static string WithTTL(int ttl) { return "ttl=" + ttl; }

        public static string WithoutEcho() { return "me=0"; }

        public static string WithLast(int last) { return "last=" + last; }

        public static string WithRetain() { return Retain; }

        private static long ToTimestamp(DateTime d)
        {
            if (d.Kind == DateTimeKind.Utc)
                return (long)d.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            return (long)d.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public static string WithFrom(DateTime from) { return "from=" + ToTimestamp(from); }

        public static string WithUntil(DateTime until) { return "until=" + ToTimestamp(until); }
    }
}
