using System;

namespace Rug.Osc.Reflection
{
    public interface IOscMemberAdapter : IDisposable
    {
        void Invoke(OscMessage message);

        void State();

        void TypeOf();

        void Usage();
    }
}