using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.PacketManipulators
{
    class PlayerFlying : PacketManipulator
    {
        public bool IsFlying;
        public override int TargetPacketID
        {
            get
            {
                return 0x1A;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            byte[] buf = new byte[1];
            if (IsFlying)
            {
                buf[0] = 0x02;
            } else
            {
                buf[0] = 0;
            }
            return buf;
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            if (PacketBody[0] == 0)
            {
                IsFlying = false;
            } else if (PacketBody[0] == 0x02)
            {
                IsFlying = true;
            }
        }
    }
}
