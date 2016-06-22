using System;
using System.Collections.Generic;

namespace Rug.Osc.Packaging
{
    public sealed class OscPackageBuilder : IOscPackageBuilder
    {
        public readonly uint QueueIdentifier;
        private const int headerSize = 0x12;
        private const int maxPackageSize = 1472;

        private readonly bool addPackageMessage;
        private readonly List<OscPacket> packets = new List<OscPacket>();

        private bool immediateMode = false;
        private ulong longPackageID;
        private int packageID = 0;
        private int packageIdentifierSize = new OscMessage(OscPackage.PackageAddress, (long)0).SizeInBytes;
        private int packageSize = 0;

        public bool ImmediateMode
        {
            get
            {
                return immediateMode;
            }

            set
            {
                Flush();

                immediateMode = value;
            }
        }

        public event Action<ulong, OscBundle> BundleComplete;

        public OscPackageBuilder(uint identifier, bool addPackageMessage)
        {
            QueueIdentifier = identifier;
            this.addPackageMessage = addPackageMessage;

            Flush();
        }

        public void Add(params OscPacket[] packets)
        {
            int size = 0;

            foreach (OscPacket packet in packets)
            {
                size = packet.SizeInBytes + 4;

                if (packageSize + size >= maxPackageSize)
                {
                    Flush();
                }

                this.packets.Add(packet);

                packageSize += size;
            }
        }

        public void AddPacketIDMessage(ulong id, bool @return)
        {
            this.packets.Add(new OscMessage(@return == false ? OscPackage.PackageAddress : OscPackage.ReturnAddress, unchecked((long)id)));

            packageSize += packageIdentifierSize + 4;
        }

        public void Flush()
        {
            if (addPackageMessage == true)
            {
                if (packets.Count == 1)
                {
                    return;
                }
            }
            else if (packets.Count == 0)
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
                packets.Clear();

                packageSize = headerSize;

                longPackageID = unchecked((((ulong)QueueIdentifier << 32) & 0xFFFFFFFF00000000) | ((ulong)packageID & 0x00000000FFFFFFFF));

                packageID++;

                if (addPackageMessage == true)
                {
                    AddPacketIDMessage(this.longPackageID, false);
                }
            }
        }
    }
}