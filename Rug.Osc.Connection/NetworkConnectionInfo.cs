using System;
using System.Net;

namespace Rug.Osc.Connection
{
    public sealed class NetworkConnectionInfo : IOscNetworkConnectionInfo
    {
        public IPAddress Address { get; private set; } 

        public string Descriptor { get; private set; }

        public IPAddress NetworkAdapterIPAddress { get; private set; }

        public int SendPort { get; private set; } 

        public int ReceivePort { get; private set; }

        public NetworkConnectionInfo(string descriptor, IPAddress networkAdapterIPAddress, IPAddress address, int sendPort, int receivePort)
        {
            Descriptor = descriptor;
            NetworkAdapterIPAddress = networkAdapterIPAddress;
            Address = address;
            SendPort = sendPort;
            ReceivePort = receivePort;
        }

        public int CompareTo(IOscConnectionInfo other)
        {
            return string.Compare(Descriptor, other.Descriptor, StringComparison.InvariantCulture);
        }

        public bool Equals(IOscConnectionInfo other)
        {
            if ((other is NetworkConnectionInfo) == false)
            {
                return false; 
            }

            NetworkConnectionInfo otherNetworkConnectionInfo = other as NetworkConnectionInfo;

            return
                Descriptor.Equals(otherNetworkConnectionInfo.Descriptor) &&
                NetworkAdapterIPAddress.Equals(otherNetworkConnectionInfo.NetworkAdapterIPAddress) &&
                Address.Equals(otherNetworkConnectionInfo.Address) &&
                SendPort.Equals(otherNetworkConnectionInfo.SendPort) && 
                ReceivePort.Equals(otherNetworkConnectionInfo.ReceivePort);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Descriptor}, Send: {Address}:{SendPort}, Receive: {NetworkAdapterIPAddress}:{ReceivePort}";
        }
    }
}