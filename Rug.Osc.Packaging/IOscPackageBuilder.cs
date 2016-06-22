namespace Rug.Osc.Packaging
{
    public interface IOscPackageBuilder
    {
        bool ImmediateMode { get; set; }

        void Add(params OscPacket[] packets);

        void Flush();
    }
}