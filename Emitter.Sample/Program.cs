using System;
using Emitter;

namespace Emitter.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var emitter = Connection.Default;

            // Connect to emitter.io service
            emitter.Connect();

            //
            emitter.GenerateKey("<secret key>", "welcome", Messages.EmitterKeyType.ReadWrite, (keygen) =>
            {
                Console.WriteLine(keygen.Key);
            });


            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
