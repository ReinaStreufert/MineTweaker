using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.PacketManipulators
{
    class EncryptionRequest : PacketManipulator
    {
        public string ServerID;
        public byte[] PublicKeyData;
        public byte[] VerifyToken;

        public override int TargetPacketID
        {
            get
            {
                return 0x01;
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
                ServerID = ms.ReadString(20);
                int publicKeyLength = ms.ReadVarInt();
                PublicKeyData = new byte[publicKeyLength];
                ms.Read(PublicKeyData, 0, publicKeyLength);
                int verifyTokenLength = ms.ReadVarInt();
                VerifyToken = new byte[verifyTokenLength];
                ms.Read(VerifyToken, 0, verifyTokenLength);
            }
        }
    }
}
