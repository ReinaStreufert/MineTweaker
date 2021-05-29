using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker
{
    public abstract class PacketManipulator
    {
        public abstract int TargetPacketID { get; }
        public abstract void ParseFromBody(byte[] PacketBody);
        public abstract byte[] GeneratePacketBody();
    }
}
