using System;
using System.Globalization;
using System.Text;
using System.Threading;
using Emitter;
using Emitter.Messages;
using Emitter.Utility;

namespace Emitter.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var channelKey = "a9R37L2dnGVJU7JQMsdbLEYpaeWnrZ0v"; // Insert your own key here.
            var channel = "test/";

            using (var emitter = Connection.Establish(channelKey, "127.0.0.1", 8080, false))
            {
                emitter.Error += (object sender, Exception e) => { Console.WriteLine("Error:" + e.Message); };
                emitter.PresenceSubscribe(channelKey, channel, false, (PresenceEvent e) => { Console.WriteLine("Presence event " + e.Event + "."); });
                emitter.PresenceStatus(channelKey, channel, (PresenceEvent e) => { Console.WriteLine("Presence event " + e.Event + "."); });

                // Handle chat messages
                emitter.Subscribe(channel,
                    (chan, msg) => { Console.WriteLine(Encoding.UTF8.GetString(msg)); },
                    Options.WithFrom(DateTime.UtcNow.AddHours(-1)),
                    Options.WithUntil(DateTime.UtcNow),
                    Options.WithLast(10_000));

                emitter.Link(channelKey, channel, "L0", true);
                emitter.PublishWithLink("L0", "Link test");

                emitter.Me += (MeResponse me) => { Console.WriteLine(me.MyId); };
                emitter.MeInfo();

                string text = "";
                Console.WriteLine("Type to chat or type 'q' to exit...");
                do
                {
                    text = Console.ReadLine();
                    emitter.Publish(channelKey, channel, text, Options.WithTTL(3600));
                }
                while (text != "q");
            }
        }

    }
}
