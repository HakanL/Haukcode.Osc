using System;

namespace Rug.Osc.Connection
{
    public class OscCommunicationException : Exception
    {
        public OscCommunicationException(string message) : base(message) { }

        public OscCommunicationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
