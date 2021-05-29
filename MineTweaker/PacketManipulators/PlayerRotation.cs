using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.PacketManipulators
{
    class PlayerRotation : PacketManipulator
    {
        public Vec2F Rotation;
        public bool OnGround;
        public override int TargetPacketID
        {
            get
            {
                return 0x14;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            byte[] buf = new byte[9];
            using (MemoryStream ms = new MemoryStream(buf))
            {
                ms.WriteNum((float)Rotation.X);
                ms.WriteNum((float)Rotation.Y);
                ms.WriteBool(OnGround);
            }
            return buf;
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            using (MemoryStream ms = new MemoryStream(PacketBody))
            {
                Rotation = new Vec2F();
                Rotation.X = ms.ReadFloat();
                Rotation.Y = ms.ReadFloat();
                OnGround = ms.ReadBool();
            }
        }
    }
}
