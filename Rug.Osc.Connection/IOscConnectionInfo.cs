namespace Rug.Osc.Connection
{
    public interface IOscConnectionInfo
    {
        string Descriptor { get; }

        int CompareTo(IOscConnectionInfo other);

        bool Equals(IOscConnectionInfo other);
    }
}