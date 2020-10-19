using System;

namespace Rug.Osc.Connection
{
    public interface IOscConnection : IDisposable
    {
        IOscConnectionInfo ConnectionInfo { get; }

        IOscAddressManager OscAddressManager { get; }

        IReporter Reporter { get; } 

        bool PackageMessages { get; } 

        void Connect();

        void Send(string oscAddress, params object[] arguments);

        void Send(params OscPacket[] packet);

        void Flush(); 
    }
}
