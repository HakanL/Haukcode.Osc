using System.Net;

namespace Rug.Osc.Connection
{
    public interface IOscNetworkConnectionInfo : IOscConnectionInfo
    {
        IPAddress Address { get; }

        IPAddress NetworkAdapterIPAddress { get; }

        int ReceivePort { get; }

        int SendPort { get;}
    }
}