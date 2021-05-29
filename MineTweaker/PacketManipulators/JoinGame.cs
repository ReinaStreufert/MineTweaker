using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.PacketManipulators
{
    class JoinGame : PacketManipulator
    {
        public int PlayerEntityID;
        public bool IsHardcore;
        public Gamemode Gamemode;
        public Gamemode PreviousGamemode; // what?
        public string[] WorldNames;
        public NbtTag DimensionCodec;
        public NbtTag Dimension;
        public string WorldName;
        public long WorldSeedHashed;
        public int MaxPlayers;
        public int ViewDistance;
        public bool ReducedDebugInfo;
        public bool EnableRespawnScreen;
        public bool IsDebugWorld;
        public bool IsFlat;
        public override int TargetPacketID
        {
            get
            {
                return 0x24;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            throw new NotImplementedException();
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            using (MemoryStream ms = new MemoryStream(PacketBody))
            {
                PlayerEntityID = ms.ReadInt();
                IsHardcore = ms.ReadBool();
                Gamemode = (Gamemode)ms.ReadByte();
                PreviousGamemode = (Gamemode)ms.ReadByte(); // i say again, what?

                int worldCount = ms.ReadVarInt();
                WorldNames = new string[worldCount];
                for (int i = 0; i < worldCount; i++)
                {
                    WorldNames[i] = ms.ReadString(32767);
                }

                DimensionCodec = NbtTag.FromStream(ms);
                Dimension = NbtTag.FromStream(ms);
                WorldName = ms.ReadString(32767);
                WorldSeedHashed = ms.ReadLong();
                MaxPlayers = ms.ReadVarInt();
                ViewDistance = ms.ReadVarInt();
                ReducedDebugInfo = ms.ReadBool();
                EnableRespawnScreen = ms.ReadBool();
                IsDebugWorld = ms.ReadBool();
                IsFlat = ms.ReadBool();
            }
        }
    }
}
