using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.PacketManipulators
{
    class ChangeGameState : PacketManipulator
    {
        public Reason ChangeReason;
        public float Argument;

        public override int TargetPacketID
        {
            get
            {
                return 0x1D;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            byte[] buf = new byte[5];
            using (MemoryStream ms = new MemoryStream(buf))
            {
                ms.WriteByte((byte)ChangeReason);
                ms.WriteNum((float)Argument);
            }
            return buf;
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            using (MemoryStream ms = new MemoryStream(PacketBody))
            {
                ChangeReason = (Reason)((byte)ms.ReadByte());
                Argument = ms.ReadFloat();
            }
        }

        public enum Reason : byte
        {
            NoRespawnBlockAvailable = 0,
            EndRaining = 1,
            BeginRaining = 2,
            ChangeGamemode = 3,
            WinGame = 4,
            DemoEvent = 5,
            ArrowHitPlayer = 6,
            RainLevelChange = 7,
            ThunderLevelChange = 8,
            PlayPufferfishStringSound = 9,
            PlayElderGuardianMobAppearance = 10,
            EnableRespawnScreen = 11
        }
    }
}
