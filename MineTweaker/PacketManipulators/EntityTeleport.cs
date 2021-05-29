using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.PacketManipulators
{
    class EntityTeleport : PacketManipulator
    {
        public int EntityID;
        public EntityPosition Position;
        public override int TargetPacketID
        {
            get
            {
                return 0x56;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            byte[] buffer = new byte[DataUtils.MeasureVarInt(EntityID) + 27];
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                ms.WriteVarInt(EntityID);
                ms.WriteNum((double)Position.Location.X);
                ms.WriteNum((double)Position.Location.Y);
                ms.WriteNum((double)Position.Location.Z);

                ms.WriteByte(Position.Yaw.Steps);
                ms.WriteByte(Position.HeadPitch.Steps);
                ms.WriteBool(Position.OnGround);
            }
            return buffer;
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            using (MemoryStream ms = new MemoryStream(PacketBody))
            {
                EntityID = ms.ReadVarInt();

                Position = new EntityPosition();
                Position.Location.X = (float)ms.ReadDouble();
                Position.Location.Y = (float)ms.ReadDouble();
                Position.Location.Z = (float)ms.ReadDouble();
                Position.Yaw = new Angle(ms.ReadByte());
                Position.HeadPitch = new Angle(ms.ReadByte());
                Position.OnGround = ms.ReadBool();
            }
        }
    }
}
