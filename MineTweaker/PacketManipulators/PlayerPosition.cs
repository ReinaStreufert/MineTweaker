using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.PacketManipulators
{
    class PlayerPosition : PacketManipulator
    {
        public Vec3D Position;
        public bool OnGround;
        public override int TargetPacketID
        {
            get
            {
                return 0x12;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            byte[] buf = new byte[25];
            using (MemoryStream ms = new MemoryStream(buf))
            {
                ms.WriteNum((double)Position.X);
                ms.WriteNum((double)Position.Y);
                ms.WriteNum((double)Position.Z);
                ms.WriteBool(OnGround);
            }
            return buf;
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            using (MemoryStream ms = new MemoryStream(PacketBody))
            {
                Position = new Vec3D();
                Position.X = ms.ReadDouble();
                Position.Y = ms.ReadDouble();
                Position.Z = ms.ReadDouble();
                OnGround = ms.ReadBool();
            }
        }
    }
}
