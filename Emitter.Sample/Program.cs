using System;
using System.Globalization;
using System.Text;
using Emitter;
using Emitter.Messages;
using Emitter.Utility;

namespace Emitter.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var channelKey = "<channel key for 'chat' channel>";

            using (var emitter = Connection.Establish())
            {
                // Generate a read-write key for our channel
                emitter.GenerateKey("<secret key>", "chat", Messages.EmitterKeyType.ReadWrite, (keygen) =>
                {
                    Console.WriteLine("Generated Key: " + keygen.Key);
                });

                // Handle chat messages
                emitter.On(channelKey, "chat", (channel, msg) =>
                {
                    Console.WriteLine(Encoding.UTF8.GetString(msg));
                });

                string text = "";
                Console.WriteLine("Type to chat or type 'q' to exit...");
                do
                {
                    text = Console.ReadLine();
                    emitter.Publish(channelKey, "chat", text);
                }
                while (text != "q");
            }
        }

    }
}
