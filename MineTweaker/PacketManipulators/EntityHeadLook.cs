﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.PacketManipulators
{
    class EntityHeadLook : PacketManipulator
    {
        public int EntityID;
        public Angle HeadYaw;

        public override int TargetPacketID
        {
            get
            {
                return 0x3A;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            byte[] buffer = new byte[DataUtils.MeasureVarInt(EntityID) + 1];
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                ms.WriteVarInt(EntityID);
                ms.WriteByte(HeadYaw.Steps);
            }
            return buffer;
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            using (MemoryStream ms = new MemoryStream(PacketBody))
            {
                EntityID = ms.ReadVarInt();
                HeadYaw = new Angle(ms.ReadByte());
            }
        }
    }
}
