# Emitter C# SDK

![](Emitter.png)


Emitter is an MQTT client tailored for [emitter.io](http://emitter.io) service, allowing real-time publish subscribe. It supports full .Net Framework, .Net Micro Framework and WinRT platform.

The current version supports several .Net Frameworks: 

* .Net Framework (2.0, 3.0, 3.5, 4.0, 4.5, 4.5.1, 4.5.2, 4.6, 4.6.1)
* .Net Micro Framework 4.2, 4.3 and 4.4
* Mono (for Linux O.S.)

The current version also supports WinRT platforms:

* Windows 8.1
* Windows Phone 8.1
* Windows 10

This library provides a nicer MQTT interface fine-tuned and extended with specific features provided by [emitter.io](http://emitter.io). The library is based upon the [M2MQTT library](http://www.m2mqtt.net
) written by [Paolo Patierno](https://m2mqtt.wordpress.com/who/) and released under Eclipse Public License.

# Build Notes
* To build for .Net Micro Framework 4.2 or 4.3, you need to download .Net Micro Framework SDK from CodePlex: https://netmf.codeplex.com/
* To build for .Net Micro Framework 4.4, you need to download .Net Micro Framework SDK from GitHub: https://github.com/NETMF/netmf-interpreter/releases

# Nuget

To install Emitter, run the following command in the [Package Manager Console](http://docs.nuget.org/consume/package-manager-console):

```
Install-Package Emitter
```

# Usage

Below is a sample program written using this C# client library. This demonstrates a simple console chat app where the messages are published to `chat` channel and every connected client receives them in real-time.

```
// Creating a connection to emitter.io service.
var emitter    = new Emitter.Connection();
var channelKey = "<channel key for 'chat' channel>";

// Connect to emitter.io service
emitter.Connect();

// Handle chat messages
emitter.On(channelKey, "chat", (channel, msg) =>
{
    Console.WriteLine(Encoding.UTF8.GetString(msg));
});

// Publish messages to the 'chat' channel
string text = "";
Console.WriteLine("Type to chat or 'q' to exit...");
do
{
    text = Console.ReadLine();
    emitter.Publish(channelKey, "chat", text);
}
while (text != "q");

// Disconnect the client
emitter.Disconnect();
```

To generate a channel key using the secret key provided, the `GenerateKey` method can be used.

```
// Generate a read-write key for our channel
emitter.GenerateKey(
    "<secret key>", 
    "chat", 
    Messages.EmitterKeyType.ReadWrite, 
    (response) => Console.WriteLine("Generated Key: " + response.Key)
    );
```
