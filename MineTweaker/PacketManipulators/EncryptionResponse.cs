using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MineTweaker.DataUtils;

namespace MineTweaker.PacketManipulators
{
    class EncryptionResponse : PacketManipulator
    {
        public byte[] EncryptedSharedSecret;
        public byte[] EncryptedVerifyToken;
        public override int TargetPacketID
        {
            get
            {
                return 0x01;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            byte[] buffer = new byte[DataUtils.MeasureVarInt(EncryptedSharedSecret.Length) + EncryptedSharedSecret.Length + DataUtils.MeasureVarInt(EncryptedVerifyToken.Length) + EncryptedVerifyToken.Length];
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                ms.WriteVarInt(EncryptedSharedSecret.Length);
                ms.Write(EncryptedSharedSecret, 0, EncryptedSharedSecret.Length);
                ms.WriteVarInt(EncryptedVerifyToken.Length);
                ms.Write(EncryptedVerifyToken, 0, EncryptedVerifyToken.Length);
            }
            return buffer;
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            throw new NotImplementedException();
        }
    }
}
