using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MineTweaker.DataUtils;

namespace MineTweaker.PacketManipulators
{
    class LoginStart : PacketManipulator
    {
        public string PlayerUsername;
        public override int TargetPacketID
        {
            get
            {
                return 0x00;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            byte[] buffer = new byte[DataUtils.MeasureString(PlayerUsername)];
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                ms.WriteString(PlayerUsername, 16);
            }
            return buffer;
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            using (MemoryStream ms = new MemoryStream(PacketBody))
            {
                PlayerUsername = ms.ReadString(16);
            }
        }
    }
}
