using System;
using Rug.Loading;

namespace Rug.Osc.Connection
{
    public static class OscConnection
    {
        [ThreadStatic]
        private static IOscConnection currentConnection;

        // TODO Remove this hack?
        public static IOscConnection DefaultConnection { get; set; }

        public static IOscConnection Current
        {
            get { return currentConnection ?? (currentConnection = DefaultConnection); }
            set { currentConnection = value; }
        }

        public static IOscConnectionInfo ConnectionInfo => Current.ConnectionInfo;

        public static IOscAddressManager OscAddressManager => Current.OscAddressManager;

        public static bool PackageMessages => Current.PackageMessages;

        public static IReporter Reporter => Current.Reporter;

        public static void Connect()
        {
            Current?.Connect(); 
        }

        public static void Dispose()
        {
            Current?.Dispose();
        }

        public static void Flush()
        {
            Current?.Flush();
        }

        public static void Send(params OscPacket[] packet)
        {
            Current?.Send(packet);
        }

        public static void Send(string oscAddress, params object[] arguments)
        {
            Current?.Send(oscAddress, arguments);
        }
    }
}
