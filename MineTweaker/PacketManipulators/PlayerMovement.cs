using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.PacketManipulators
{
    class PlayerMovement : PacketManipulator
    {
        public bool OnGround;
        public override int TargetPacketID
        {
            get
            {
                return 0x15;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            byte[] buf = new byte[1];
            using (MemoryStream ms = new MemoryStream(buf))
            {
                ms.WriteBool(OnGround);
            }
            return buf;
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            using (MemoryStream ms = new MemoryStream(PacketBody))
            {
                OnGround = ms.ReadBool();
            }
        }
    }
}
