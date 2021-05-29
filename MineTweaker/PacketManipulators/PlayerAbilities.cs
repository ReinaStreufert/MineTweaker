using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.PacketManipulators
{
    public class PlayerAbilities : PacketManipulator
    {
        public Ability Abilities;
        public float FlyingSpeed = 0.05F; // idk why tf this is even here tbh. why not make it an attribute???
        public float FOV = 0.1F;

        public override int TargetPacketID
        {
            get
            {
                return 0x30;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            byte[] buf = new byte[9];
            using (MemoryStream ms = new MemoryStream(buf))
            {
                ms.WriteByte((byte)Abilities);
                ms.WriteNum((float)FlyingSpeed);
                ms.WriteNum((float)FOV);
            }
            return buf;
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            using (MemoryStream ms = new MemoryStream(PacketBody))
            {
                Abilities = (Ability)ms.ReadByte();
                FlyingSpeed = ms.ReadFloat();
                FOV = ms.ReadFloat();
            }
        }
        [Flags]
        public enum Ability : byte
        {
            Invulnerable = 0x01,
            Flying = 0x02,
            AllowFlying = 0x04,
            InstantBreak = 0x08
        }
    }
}
