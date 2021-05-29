using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MineTweaker.DataUtils;

namespace MineTweaker.PacketManipulators
{
    class SetCompression : PacketManipulator
    {
        public int Threshold;
        public override int TargetPacketID
        {
            get
            {
                return 0x03;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            byte[] buffer = new byte[DataUtils.MeasureVarInt(Threshold)];
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                ms.WriteVarInt(Threshold);
            }
            return buffer;
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            using (MemoryStream ms = new MemoryStream(PacketBody))
            {
                Threshold = ms.ReadVarInt();
            }
        }
    }
}
