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
            //var channelKey = "LbrBtLmdqF-qyMQpFhfTBij9n7LwYnRT";
            var channelKey = "EckDAy4LHt_T0eTPSBK_0dmOAGhakMgI";//local

            //var channelKey = "EckDAy4LHt_T0eTPSBK_0dmOAGhakMgJ"; # fake, to generate error

            var channel = "test/";
            var shareGroup = "sg";
            var shareGroupKey = "qQrtann17qNi3CTwW7N8F4OR9uAuQBHw";//local

            using (var emitter = Connection.Establish("127.0.0.1", 8080, channelKey))
            {
                /*
                // Generate a read-write key for our channel
                emitter.GenerateKey("<secret key>", "chat", Messages.SecurityAccess.ReadWrite, (keygen) =>
                {
                    Console.WriteLine("Generated Key: " + keygen.Key);
                });
                */

                //emitter.Presence += (PresenceEvent e) => { Console.WriteLine("Presence event " + e.Event + "."); };
                //emitter.PresenceSubscription(channelKey, "chat", true);
                emitter.Error += (object sender, Exception e) => { Console.WriteLine("Error:" + e.Message); };
                emitter.PresenceSubscribe(channelKey, channel, false, (PresenceEvent e) => { Console.WriteLine("Presence event " + e.Event + "."); });
                emitter.PresenceStatus(channelKey, channel, (PresenceEvent e) => { Console.WriteLine("Presence event " + e.Event + "."); });
                Thread.Sleep(1000);
                // Handle chat messages
                /*
                emitter.On(channelKey, channel, (chan, msg) =>
                {
                    Console.WriteLine(Encoding.UTF8.GetString(msg));
                }, 5);
                */
                emitter.SubscribeWithGroup(shareGroupKey, channel, shareGroup,
                    (chan, msg) => { Console.WriteLine(Encoding.UTF8.GetString(msg)); });

                emitter.Link(channelKey, channel, "L0", false, true);
                emitter.Link(channelKey, channel, "L1", false, true);
                emitter.PublishWithLink("L0", "Link test");

                emitter.Me += (MeResponse me) => { Console.WriteLine(me.MyId); };
                emitter.MeInfo();

                string text = "";
                Console.WriteLine("Type to chat or type 'q' to exit...");
                do
                {
                    text = Console.ReadLine();
                    //emitter.Publish(channelKey, "chat", text, Options.WithRetain());
                    emitter.Publish(channelKey, channel, text);
                }
                while (text != "q");
            }
        }

    }
}
