using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using Rug.Osc;

namespace ConnectTest
{
	class Program
	{
		static void Main(string[] args)
		{
			IPAddress remote = IPAddress.Parse("192.168.2.4"); 
			int remotePort = 5000; 


			if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
			{
				Console.WriteLine("No network connections available");
				return; 
			}

			// Get host name
			String strHostName = Dns.GetHostName();

			// Find host by name
			IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);
			
			NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

			foreach (NetworkInterface adapter in interfaces)
			{
				var ipProps = adapter.GetIPProperties();

				foreach (var ip in ipProps.UnicastAddresses)
				{
					if ((adapter.OperationalStatus == OperationalStatus.Up)
						&& (ip.Address.AddressFamily == AddressFamily.InterNetwork))
					{
						Console.WriteLine(ip.Address.ToString().PadRight(20) + adapter.Description.ToString());

						try
						{
							using (OscSender sender = new OscSender(ip.Address, remote, remotePort))
							{
								sender.Connect();

								sender.Send(new OscMessage("/test", 1, 2, 3));
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Exception while sending");
							Console.WriteLine(ex.Message);
						}

						Console.WriteLine(); 
					}
				}
			}

			Console.ReadKey(true); 
		}
	}
}
