using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MineTweaker.DataUtils;

namespace MineTweaker.PacketManipulators
{
    class Handshake : PacketManipulator
    {
        public int ProtocolVersion;
        public string ServerAddress;
        public int ServerPort;
        public ConnectionState NextState;

        public override int TargetPacketID
        {
            get
            {
                return 0x00;
            }
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            using (MemoryStream ms = new MemoryStream(PacketBody))
            {
                ProtocolVersion = ms.ReadVarInt();
                ServerAddress = ms.ReadString(255);
                ServerPort = ms.ReadUshort();
                NextState = (ConnectionState)(ms.ReadVarInt() + 1);
                if (NextState != ConnectionState.Status && NextState != ConnectionState.Login)
                {
                    throw new ArgumentException("Next state must be status or login");
                }
            }
        }

        public override byte[] GeneratePacketBody()
        {
            byte[] buffer = new byte[DataUtils.MeasureVarInt(ProtocolVersion) + DataUtils.MeasureString(ServerAddress) + 2 + DataUtils.MeasureVarInt((int)NextState - 1)];
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                ms.WriteVarInt(ProtocolVersion);
                ms.WriteString(ServerAddress, 255);
                ms.WriteNum((ushort)ServerPort);
                ms.WriteVarInt((int)NextState - 1);
            }
            return buffer;
        }
    }
}
