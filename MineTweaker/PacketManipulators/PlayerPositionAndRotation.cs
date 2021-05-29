using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.PacketManipulators
{
    class PlayerPositionAndRotation : PacketManipulator
    {
        public Vec3D Position;
        public Vec2F Rotation;
        public bool OnGround;
        public override int TargetPacketID
        {
            get
            {
                return 0x13;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            byte[] buf = new byte[33];
            using (MemoryStream ms = new MemoryStream(buf))
            {
                ms.WriteNum((double)Position.X);
                ms.WriteNum((double)Position.Y);
                ms.WriteNum((double)Position.Z);
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
                Position = new Vec3D();
                Rotation = new Vec2F();
                Position.X = ms.ReadDouble();
                Position.Y = ms.ReadDouble();
                Position.Z = ms.ReadDouble();
                Rotation.X = ms.ReadFloat();
                Rotation.Y = ms.ReadFloat();
                OnGround = ms.ReadBool();
            }
        }
    }
}
