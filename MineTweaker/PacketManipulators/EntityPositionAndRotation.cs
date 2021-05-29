using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.PacketManipulators
{
    class EntityPositionAndRotation : PacketManipulator // Entity head look is required for head yaw changes, use EntityTeleport if the entity moves more than 8 blocks on 1 or more axis
    {
        public int EntityID;
        public EntityPosition OldPosition; // When calling GeneratePacketBody, fill in both OldPosition and NewPosition, unusually however, fill in OldPosition before calling ParseFromBody, which will then determine NewPosition.
        public EntityPosition NewPosition;
        public override int TargetPacketID
        {
            get
            {
                return 0x28;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            if (Math.Abs(NewPosition.Location.X - OldPosition.Location.X) > 8 || Math.Abs(NewPosition.Location.Y - OldPosition.Location.Y) > 8 || Math.Abs(NewPosition.Location.Z - OldPosition.Location.Z) > 8)
            {
                throw new ArgumentException("Entity moved more than 8 blocks on at least 1 axis. Use EntityTeleport instead.");
            }
            byte[] buffer = new byte[DataUtils.MeasureVarInt(EntityID) + 9];
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                ms.WriteVarInt(EntityID);
                ms.WriteNum((short)((NewPosition.Location.X * 32F - OldPosition.Location.X * 32F) * 128F));
                ms.WriteNum((short)((NewPosition.Location.Y * 32F - OldPosition.Location.Y * 32F) * 128F));
                ms.WriteNum((short)((NewPosition.Location.Z * 32F - OldPosition.Location.Z * 32F) * 128F));
                ms.WriteByte(NewPosition.Yaw.Steps);
                ms.WriteByte(NewPosition.HeadPitch.Steps);
                ms.WriteBool(NewPosition.OnGround);
            }
            return buffer;
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            using (MemoryStream ms = new MemoryStream(PacketBody))
            {
                EntityID = ms.ReadVarInt();
                short encodedDeltaX = ms.ReadShort();
                short encodedDeltaY = ms.ReadShort();
                short encodedDeltaZ = ms.ReadShort();
                float deltaX = (float)encodedDeltaX / (32F * 128F);
                float deltaY = (float)encodedDeltaY / (32F * 128F);
                float deltaZ = (float)encodedDeltaZ / (32F * 128F);

                NewPosition = new EntityPosition();
                NewPosition.Location.X = OldPosition.Location.X + deltaX;
                NewPosition.Location.Y = OldPosition.Location.Y + deltaY;
                NewPosition.Location.Z = OldPosition.Location.Z + deltaZ;

                NewPosition.Yaw = new Angle(ms.ReadByte());
                NewPosition.HeadPitch = new Angle(ms.ReadByte());

                NewPosition.OnGround = ms.ReadBool();
            }
        }
    }
}
