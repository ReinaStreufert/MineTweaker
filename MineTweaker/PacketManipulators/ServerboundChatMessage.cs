using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.PacketManipulators
{
    class ServerboundChatMessage : PacketManipulator
    {
        public string Message;
        public override int TargetPacketID
        {
            get
            {
                return 0x03;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            byte[] buffer = new byte[DataUtils.MeasureString(Message)];
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                ms.WriteString(Message, 256);
            }
            return buffer;
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            using (MemoryStream ms = new MemoryStream(PacketBody))
            {
                Message = ms.ReadString(256);
            }
        }
    }
}
