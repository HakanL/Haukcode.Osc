using System;
using System.Collections.Generic;

namespace Haukcode.Osc.Packaging
{
    public delegate void OscPackegeEvent(ulong packageID, OscBundle bundle);

    public sealed class OscPackageBuilder : IOscPackageBuilder
    {
        public readonly uint QueueIdentifier;

        private const int headerSize = 0x12;
        private const int maxPackageSize = 1472;

        private readonly List<OscPacket> packets = new List<OscPacket>();

        private OscPackageBuilderMode oscPackageBuilderMode = OscPackageBuilderMode.Bundled;

        private ulong longPackageID;
        private int packageID = 0;
        private int packageIdentifierSize = new OscMessage(OscPackage.PackageAddress, (long)0).SizeInBytes;
        private int packageSize = 0;

        private readonly object addSyncLock = new object(); 

        public OscPackageBuilderMode Mode
        {
            get => oscPackageBuilderMode;

            set
            {
                if (value == oscPackageBuilderMode)
                {
                    return;
                }

                if (oscPackageBuilderMode == OscPackageBuilderMode.PackagedAndQueued)
                {
                    Flush();
                }

                OscPackageBuilderMode oldMode = oscPackageBuilderMode;

                oscPackageBuilderMode = value;

                if ((oldMode != OscPackageBuilderMode.Packaged && oldMode != OscPackageBuilderMode.PackagedAndQueued) &&
                    (value == OscPackageBuilderMode.Packaged || value == OscPackageBuilderMode.PackagedAndQueued))
                {
                    AddPacketHeader();
                }
            }
        }

        public event OscPackegeEvent BundleComplete;

        public OscPackageBuilder(uint identifier)
        {
            QueueIdentifier = identifier;
        }

        public void Add(params OscPacket[] packets)
        {
            lock (addSyncLock)
            {
                foreach (OscPacket packet in packets)
                {
                    if (oscPackageBuilderMode == OscPackageBuilderMode.Immediate)
                    {
                        BundleComplete?.Invoke(longPackageID, packet is OscBundle ? packet as OscBundle : new OscBundle(DateTime.Now, packet));

                        continue;
                    }

                    int size = packet.SizeInBytes + 4;

                    if (packageSize + size >= maxPackageSize)
                    {
                        Flush();
                    }

                    this.packets.Add(packet);

                    packageSize += size;
                }
            }
        }

        public void AddPacketIDMessage(ulong id, bool @return)
        {
            lock (addSyncLock)
            {
                if (oscPackageBuilderMode == OscPackageBuilderMode.Immediate ||
                    oscPackageBuilderMode == OscPackageBuilderMode.Bundled)
                {
                    return;
                }

                this.packets.Add(CreatePacketIDMessage(id, @return));

                packageSize += packageIdentifierSize + 4;
            }
        }

        public void Flush()
        {
            lock (addSyncLock)
            {
                if (oscPackageBuilderMode == OscPackageBuilderMode.Immediate)
                {
                    return;
                }

                if (oscPackageBuilderMode != OscPackageBuilderMode.Bundled)
                {
                    if (packets.Count == 1)
                    {
                        return;
                    }

                    if (packets.Count == 0)
                    {
                        AddPacketHeader();
                        return;
                    }
                }

                if (packets.Count == 0)
                {
                    return;
                }

                try
                {
                    OscBundle bundle = new OscBundle(DateTime.Now, packets.ToArray());

                    BundleComplete?.Invoke(longPackageID, bundle);
                }
                finally
                {
                    AddPacketHeader();
                }
            }
        }

        private void AddPacketHeader()
        {
            packets.Clear();

            packageSize = headerSize;

            longPackageID = unchecked((((ulong)QueueIdentifier << 32) & 0xFFFFFFFF00000000) | ((ulong)packageID & 0x00000000FFFFFFFF));

            packageID++;

            AddPacketIDMessage(this.longPackageID, false);
        }

        public static OscMessage CreatePacketIDMessage(ulong id, bool @return)
        {
            return new OscMessage(@return == false ? OscPackage.PackageAddress : OscPackage.ReturnAddress, unchecked((long)id));
        }
    }
}