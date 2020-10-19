using System;

namespace Haukcode.Osc.Packaging
{
    [Serializable]
    public class InnerThreadException : Exception
    {
        public InnerThreadException(string message) : base(message)
        {
        }

        public InnerThreadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}