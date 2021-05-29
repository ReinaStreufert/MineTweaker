using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.PacketManipulators
{
    class ClientboundChatMessage : PacketManipulator
    {
        public Chat Message;
        public override int TargetPacketID
        {
            get
            {
                return 0x0E;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            string json = Message.ToJSON().ToString();
            byte[] buffer = new byte[DataUtils.MeasureString(json) + 17];
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                ms.WriteString(json, 32767);
                ms.WriteByte(0); // Chat box
                ms.WriteNum((long)0); // UUID part 1 (show no matter what)
                ms.WriteNum((long)0); // UUID part 2
            }
            return buffer;
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            throw new NotImplementedException();
        }
    }
}
