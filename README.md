# Haukcode.Osc [![NuGet Version](http://img.shields.io/nuget/v/Haukcode.Osc.svg?style=flat)](https://www.nuget.org/packages/Haukcode.Osc/)

Simple, complete, open source OSC implementation for .NET Standard 2.0

*Fork from [Rug.Osc](https://bitbucket.org/rugcode/rug.osc) which hasn't seen updates in several years*

# Key Features: 
* Pure .NET (C#) Standard 2.0
* Message argument types supported are: **int**, **long**, **float**, **double**, **string**, **symbol**, **bool**, **RGBA**, **Osc-Null**, **Osc-Timetag**, **Osc-Midi**, **impulse**, **char**, **blob** and **arrays**.
* Message address patterns and pattern matching.
* Osc message bundles.
* Send and receive osc packets via UDP.
* Supports IPv4, IPv6, Unicast, Multicast and Broadcast (IPv4 only)
* Read and write osc packets to streams.
* Parse osc messages and bundles from strings.
* Optimized for use in real-time applications.
* 100% thread safe.
* Released under a permissive MIT License.




# Send Example

```
var address = IPAddress.Parse("127.0.0.1"); 
int port = 12345;

using (var sender = new OscSender(address, port)) 
{
	sender.Connect();

	sender.Send(new OscMessage("/test", 1, 2, 3, 4));
}

```


# More Examples
* [Sending a message](https://bitbucket.org/rugcode/rug.osc/wiki/Sending%20a%20message)
* [Receiving a message](https://bitbucket.org/rugcode/rug.osc/wiki/Receiving%20a%20message)
