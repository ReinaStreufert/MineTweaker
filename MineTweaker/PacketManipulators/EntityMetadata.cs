using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.PacketManipulators
{
    class EntityMetadata : PacketManipulator
    {
        public int EntityID;
        public MetadataEntry[] ChangedMetadata;
        public override int TargetPacketID
        {
            get
            {
                return 0x44;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            int len = DataUtils.MeasureVarInt(EntityID);
            for (int i = 0; i < ChangedMetadata.Length; i++)
            {
                len += 1 + DataUtils.MeasureVarInt(ChangedMetadata[i].TypeID) + ChangedMetadata[i].Data.Length;
            }
            len++;

            byte[] buffer = new byte[len];
            using (MemoryStream ms = new MemoryStream())
            {
                ms.WriteVarInt(EntityID);
                for (int i = 0; i < ChangedMetadata.Length; i++)
                {
                    ms.WriteByte((byte)ChangedMetadata[i].FieldID);
                    ms.WriteVarInt(ChangedMetadata[i].TypeID);
                    ms.Write(ChangedMetadata[i].Data, 0, ChangedMetadata[i].Data.Length);
                }
                ms.WriteByte(0xFF);
            }
            return buffer;
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            using (MemoryStream ms = new MemoryStream(PacketBody))
            {
                EntityID = ms.ReadVarInt();
                List<MetadataEntry> entries = new List<MetadataEntry>();
                byte fieldID = (byte)ms.ReadByte();
                while (fieldID != 0xFF)
                {
                    MetadataEntry entry = new MetadataEntry();
                    entry.FieldID = fieldID;
                    entry.TypeID = ms.ReadVarInt();

                    int dataLength = 0;
                    int startIndex;
                    bool optional;
                    switch ((MetadataTypes)entry.TypeID)
                    {
                        case MetadataTypes.Byte:
                            dataLength = 1;
                            break;
                        case MetadataTypes.VarInt:
                            startIndex = (int)ms.Position;
                            ms.ReadVarInt();
                            dataLength = (int)ms.Position - startIndex;
                            ms.Seek(-dataLength, SeekOrigin.Current);
                            break;
                        case MetadataTypes.Float:
                            dataLength = 4;
                            break;
                        case MetadataTypes.String:
                            startIndex = (int)ms.Position;
                            ms.ReadString(int.MaxValue);
                            dataLength = (int)ms.Position - startIndex;
                            ms.Seek(-dataLength, SeekOrigin.Current);
                            break;
                        case MetadataTypes.Chat:
                            startIndex = (int)ms.Position;
                            ms.ReadString(int.MaxValue);
                            dataLength = (int)ms.Position - startIndex;
                            ms.Seek(-dataLength, SeekOrigin.Current);
                            break;
                        case MetadataTypes.OptChat:
                            startIndex = (int)ms.Position;
                            optional = ms.ReadBool();
                            if (optional)
                            {
                                ms.ReadString(int.MaxValue);
                            }
                            dataLength = (int)ms.Position - startIndex;
                            ms.Seek(-dataLength, SeekOrigin.Current);
                            break;
                        case MetadataTypes.Slot:
                            startIndex = (int)ms.Position;
                            new Slot(ms);
                            dataLength = (int)ms.Position - startIndex;
                            ms.Seek(-dataLength, SeekOrigin.Current);
                            break;
                        case MetadataTypes.Boolean:
                            dataLength = 1;
                            break;
                        case MetadataTypes.Rotation:
                            dataLength = 4 + 4 + 4;
                            break;
                        case MetadataTypes.Position:
                            dataLength = 8;
                            break;
                        case MetadataTypes.OptPosition:
                            if (ms.ReadBool())
                            {
                                dataLength = 9;
                            } else
                            {
                                dataLength = 1;
                            }
                            ms.Seek(-1, SeekOrigin.Current);
                            break;
                        case MetadataTypes.Direction:
                            startIndex = (int)ms.Position;
                            ms.ReadVarInt();
                            dataLength = (int)ms.Position - startIndex;
                            ms.Seek(-dataLength, SeekOrigin.Current);
                            break;
                        case MetadataTypes.OptUUID:
                            if (ms.ReadBool())
                            {
                                dataLength = 17;
                            }
                            else
                            {
                                dataLength = 1;
                            }
                            ms.Seek(-1, SeekOrigin.Current);
                            break;
                        case MetadataTypes.OptBlockID:
                            startIndex = (int)ms.Position;
                            ms.ReadVarInt();
                            dataLength = (int)ms.Position - startIndex;
                            ms.Seek(-dataLength, SeekOrigin.Current);
                            break;
                        case MetadataTypes.NBT:
                            startIndex = (int)ms.Position;
                            NbtTag.FromStream(ms);
                            dataLength = (int)ms.Position - startIndex;
                            ms.Seek(-dataLength, SeekOrigin.Current);
                            break;
                        case MetadataTypes.Particle:
                            // im gonna fucking cry. why did they do this lmfao i dont wanna implement all these types why didnt they add a fucking length field im gonna cryyyyyy
                            // im really tired and just want this work, ill write an actual particle class later maybe idk who am i kidding i wont need it lmaooo
                            startIndex = (int)ms.Position;
                            int particleID = ms.ReadVarInt();
                            if (particleID == 3) // minecraft:block
                            {
                                ms.ReadVarInt(); // BlockState
                            } else if (particleID == 14) // minecraft:dust
                            {
                                ms.Seek(16, SeekOrigin.Current); // 4 floats
                            } else if (particleID == 23) // minecraft:falling_dust
                            {
                                ms.ReadVarInt(); // BlockState
                            } else if (particleID == 32) // minecraft:item
                            {
                                Slot slot = new Slot(ms); // Item
                            }
                            // ok that wasnt as bad as i thought it'd be lmaoooo
                            dataLength = (int)ms.Position - startIndex;
                            ms.Seek(-dataLength, SeekOrigin.Current);
                            break;
                        case MetadataTypes.VillagerData:
                            startIndex = (int)ms.Position;
                            for (int i = 0; i < 3; i++)
                            {
                                ms.ReadVarInt();
                            }
                            dataLength = (int)ms.Position - startIndex;
                            ms.Seek(-dataLength, SeekOrigin.Current);
                            break;
                        case MetadataTypes.OptVarInt:
                            startIndex = (int)ms.Position;
                            ms.ReadVarInt();
                            dataLength = (int)ms.Position - startIndex;
                            ms.Seek(-dataLength, SeekOrigin.Current);
                            break;
                        case MetadataTypes.Pose:
                            startIndex = (int)ms.Position;
                            ms.ReadVarInt();
                            dataLength = (int)ms.Position - startIndex;
                            ms.Seek(-dataLength, SeekOrigin.Current);
                            break;
                    }
                    if (dataLength == 0)
                    {
                        throw new FormatException("Unknown metadata type");
                    } else
                    {
                        entry.Data = new byte[dataLength];
                        ms.Read(entry.Data, 0, dataLength);
                    }
                    entries.Add(entry);
                    fieldID = (byte)ms.ReadByte();
                }
                ChangedMetadata = entries.ToArray(); // oh thank god its over im crying tears of happiness
            }
        }

        private enum MetadataTypes : int
        {
            Byte = 0,
            VarInt = 1,
            Float = 2,
            String = 3,
            Chat = 4,
            OptChat = 5,
            Slot = 6,
            Boolean = 7,
            Rotation = 8,
            Position = 9,
            OptPosition = 10,
            Direction = 11,
            OptUUID = 12,
            OptBlockID = 13,
            NBT = 14,
            Particle = 15,
            VillagerData = 16,
            OptVarInt = 17,
            Pose = 18
        }

        public struct MetadataEntry
        {
            public int FieldID;
            public int TypeID;
            public byte[] Data;
        }
    }
}
