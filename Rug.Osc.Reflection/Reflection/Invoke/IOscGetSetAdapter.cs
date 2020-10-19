namespace Rug.Osc.Reflection
{
    public interface IOscGetSetAdapter : IOscMemberAdapter
    {
        bool CanRead { get; }

        bool CanWrite { get; }

        object Get();

        void Set(object value);
    }
}