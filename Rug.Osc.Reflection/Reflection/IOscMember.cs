using System.Reflection;

namespace Rug.Osc.Reflection
{
    public interface IOscMember
    {
        MemberInfo MemberInfo { get; }

        string MemberName { get; }

        string OscAddress { get; }

        OscType OscType { get; }

        string Usage { get; }

        void Describe();
    }
}