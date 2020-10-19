namespace Haukcode.Osc.Packaging
{
    public enum OscPackageBuilderMode
    {
        Immediate,
        Bundled,
        Packaged,
        PackagedAndQueued,
    }

    public interface IOscPackageBuilder
    {
        OscPackageBuilderMode Mode { get; set; }

        void Add(params OscPacket[] packets);

        void Flush();
    }
}