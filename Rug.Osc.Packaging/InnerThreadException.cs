using System;

namespace Rug.Osc.Packaging
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