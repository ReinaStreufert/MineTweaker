using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.PacketManipulators
{
    class EntityEffect : PacketManipulator
    {
        public int EntityID;
        public Effect Effect;
        public byte Amplifier;
        public int Duration;
        public EffectFlags Flags;

        public override int TargetPacketID
        {
            get
            {
                return 0x59;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            byte[] buffer = new byte[DataUtils.MeasureVarInt(EntityID) + DataUtils.MeasureVarInt(Duration) + 3];
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                ms.WriteVarInt(EntityID);
                ms.WriteByte((byte)Effect);
                ms.WriteByte(Amplifier);
                ms.WriteVarInt(Duration);
                ms.WriteByte((byte)Flags);
            }
            return buffer;
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            using (MemoryStream ms = new MemoryStream(PacketBody))
            {
                EntityID = ms.ReadVarInt();
                Effect = (Effect)ms.ReadByte();
                Amplifier = (byte)ms.ReadByte();
                Duration = ms.ReadVarInt();
                Flags = (EffectFlags)ms.ReadByte();
            }
        }

        [Flags]
        public enum EffectFlags : byte
        {
            IsAmbient = 0x01,
            ShowParticles = 0x02,
            ShowIcon = 0x04
        }
    }
}
