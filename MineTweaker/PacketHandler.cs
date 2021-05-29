using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker
{
    public abstract class PacketHandler
    {
        public abstract bool HandlePacket(Packet Packet, PacketDirection Direction, Relay Relay); // Return value is whether or not to deliver the packet.
    }
}