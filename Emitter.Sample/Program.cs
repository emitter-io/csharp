using System;
using System.Text;
using Emitter;

namespace Emitter.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var emitter = Connection.Default;
            var channelKey = "<channel key for 'chat' channel>";

            // Connect to emitter.io service
            emitter.Connect();

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

            // Disconnect the client
            emitter.Disconnect();
        }
    }
}
