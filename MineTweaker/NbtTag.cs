using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker
{
    public class NbtTag
    {
        private readonly static Type[] valueDictionary = new Type[]
        {
            null, // end
            typeof(short), // short
            typeof(int), // int
            typeof(long), // long
            typeof(float), // float
            typeof(double), // double
            typeof(byte[]), // byte array
            typeof(string), // string
            null, // list
            null, // compound
            typeof(int[]), // int array
            typeof(long[]) // long array
        };
        public static bool NbtValueTypeToCSharpType(NbtTagType TagType, out Type CSharpType)
        {
            CSharpType = valueDictionary[(byte)TagType];
            if (CSharpType != null)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public NbtTagType Type { get; private set; }
        public NbtTagType Subtype
        {
            get
            {
                if (Type == NbtTagType.List)
                {
                    return subtype;
                } else
                {
                    throw new InvalidOperationException("Subtype is only available for List tags");
                }
            }
        }
        public object Value
        {
            get
            {
                if (valueType)
                {
                    return value;
                } else
                {
                    throw new InvalidOperationException("Tag is not a value type");
                }
            }
        }
        public NbtTag this[object Indexer]
        {
            get
            {
                if (Type == NbtTagType.List)
                {
                    if (Indexer.GetType().IsAssignableFrom(typeof(int)))
                    {
                        int i = (int)Indexer;
                        if (i >= indices.Count)
                        {
                            throw new IndexOutOfRangeException("Index out of range");
                        }
                        return indices[i].Value;
                    } else
                    {
                        throw new InvalidOperationException("List tags must be indexed with an integer");
                    }
                } else if (Type == NbtTagType.Compound)
                {
                    if (Indexer.GetType() == typeof(string))
                    {
                        string i = (string)Indexer;
                        foreach (Index index in indices)
                        {
                            if (index.Name == i)
                            {
                                return index.Value;
                            }
                        }
                        throw new KeyNotFoundException("Index was not found");
                    } else
                    {
                        throw new InvalidOperationException("Compound tags must be indexed with a string");
                    }
                } else
                {
                    throw new InvalidOperationException("Tag is not an index type");
                }
            }
            set
            {
                if (Type == NbtTagType.List)
                {
                    if (subtype != value.Type)
                    {
                        throw new ArgumentException("Tag must match subtype of list tag");
                    }
                    if (Indexer.GetType().IsAssignableFrom(typeof(int)))
                    {
                        int i = (int)Indexer;
                        if (i >= indices.Count)
                        {
                            throw new IndexOutOfRangeException("Index out of range");
                        }
                        Index ind = indices[i];
                        ind.Value = value;
                    }
                    else
                    {
                        throw new InvalidOperationException("List tags must be indexed with an integer");
                    }
                }
                else if (Type == NbtTagType.Compound)
                {
                    if (Indexer.GetType() == typeof(string))
                    {
                        string i = (string)Indexer;
                        foreach (Index index in indices)
                        {
                            if (index.Name == i)
                            {
                                Index ind = index;
                                ind.Value = value;
                            }
                        }
                        throw new KeyNotFoundException("Index was not found");
                    }
                    else
                    {
                        throw new InvalidOperationException("Compound tags must be indexed with a string");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Tag is not an index type");
                }
            }
        }
        public int IndexCount
        {
            get
            {
                if (!valueType)
                {
                    return indices.Count;
                } else
                {
                    throw new InvalidOperationException("Tag is not an index type");
                }
            }
        }
        public IEnumerable<string> IndexNames
        {
            get
            {
                if (Type == NbtTagType.Compound)
                {
                    foreach (Index index in indices)
                    {
                        yield return index.Name;
                    }
                } else
                {
                    throw new InvalidOperationException("Tag is not a compound tag");
                }
            }
        }
        public T GetValue<T>()
        {
            if (!valueType)
            {
                throw new InvalidOperationException("Tag is not a value type");
            }
            Type csharpType;
            NbtValueTypeToCSharpType(Type, out csharpType);
            if (typeof(T) == csharpType)
            {
                return (T)value;
            } else
            {
                throw new InvalidCastException("Tag is not of the correct type");
            }
        }

        private NbtTagType subtype;
        private object value;
        private bool valueType;
        private List<Index> indices;

        private class Index
        {
            public string Name;
            public NbtTag Value = null;
        }
        private NbtTag() { }

        public static NbtTag CreateValueTag(NbtTagType TagType, object Value)
        {
            Type csharptype;
            if (NbtValueTypeToCSharpType(TagType, out csharptype))
            {
                if (Value.GetType() == csharptype)
                {
                    NbtTag nbtTag = new NbtTag();
                    nbtTag.Type = TagType;
                    nbtTag.value = Value;
                    nbtTag.valueType = true;
                    return nbtTag;
                } else
                {
                    throw new ArgumentException("Tag type does not match value type");
                }
            } else
            {
                throw new ArgumentException("Tag type is not a value type");
            }
        }
        public static NbtTag CreateCompoundTag()
        {
            NbtTag nbtTag = new NbtTag();
            nbtTag.Type = NbtTagType.Compound;
            nbtTag.valueType = false;
            nbtTag.indices = new List<Index>();
            return nbtTag;
        }
        public static NbtTag CreateListTag(NbtTagType Subtype, int ElementCount)
        {
            NbtTag nbtTag = new NbtTag();
            nbtTag.Type = NbtTagType.List;
            nbtTag.valueType = false;
            nbtTag.subtype = Subtype;
            nbtTag.indices = new List<Index>();
            for (int i = 0; i < ElementCount; i++)
            {
                nbtTag.indices.Add(new Index());
            }
            return nbtTag;
        }
        public static NbtTag FromStream(Stream NbtStream)
        {
            byte typeID = (byte)NbtStream.ReadByte();
            return readPayload(NbtStream, typeID);
        }
        private static NbtTag readPayload(Stream NbtStream, byte typeID)
        {
            if (typeID > 12)
            {
                throw new InvalidDataException("NBT is invalid.");
            } else if (typeID == 0)
            {
                return CreateCompoundTag();
            }
            switch (typeID)
            {
                case (byte)NbtTagType.Byte:
                    return CreateValueTag(NbtTagType.Byte, (byte)NbtStream.ReadByte());
                case (byte)NbtTagType.Short:
                    return CreateValueTag(NbtTagType.Short, NbtStream.ReadShort());
                case (byte)NbtTagType.Int:
                    return CreateValueTag(NbtTagType.Int, NbtStream.ReadInt());
                case (byte)NbtTagType.Long:
                    return CreateValueTag(NbtTagType.Long, NbtStream.ReadLong());
                case (byte)NbtTagType.Float:
                    return CreateValueTag(NbtTagType.Float, NbtStream.ReadFloat());
                case (byte)NbtTagType.Double:
                    return CreateValueTag(NbtTagType.Double, NbtStream.ReadDouble());
                case (byte)NbtTagType.ByteArray:
                    int byteArrayLen = NbtStream.ReadInt();
                    byte[] bytes = new byte[byteArrayLen];
                    NbtStream.Read(bytes, 0, byteArrayLen);
                    return CreateValueTag(NbtTagType.ByteArray, bytes);
                case (byte)NbtTagType.String:
                    ushort stringLen = NbtStream.ReadUshort();
                    byte[] stringBytes = new byte[stringLen];
                    NbtStream.Read(stringBytes, 0, stringLen);
                    string str = Encoding.UTF8.GetString(stringBytes);
                    return CreateValueTag(NbtTagType.String, str);
                case (byte)NbtTagType.List:
                    byte listType = (byte)NbtStream.ReadByte();
                    int listLength = NbtStream.ReadInt();
                    if (listLength > 0 && typeID == 0)
                    {
                        throw new InvalidDataException("NBT is invalid.");
                    }
                    if (typeID > 12)
                    {
                        throw new InvalidDataException("NBT is invalid.");
                    }
                    NbtTag list = CreateListTag((NbtTagType)listType, listLength);
                    for (int i = 0; i < listLength; i++)
                    {
                        list[i] = readPayload(NbtStream, listType);
                    }
                    return list;
                case (byte)NbtTagType.Compound:
                    NbtTag compound = CreateCompoundTag();
                    for (;;)
                    {
                        byte itemTypeID = (byte)NbtStream.ReadByte();
                        if (itemTypeID == 0)
                        {
                            break;
                        } else
                        {
                            ushort nameLen = NbtStream.ReadUshort();
                            byte[] nameBytes = new byte[nameLen];
                            NbtStream.Read(nameBytes, 0, nameLen);
                            string name = Encoding.UTF8.GetString(nameBytes);
                            compound[name] = readPayload(NbtStream, itemTypeID);
                        }
                    }
                    return compound;
                case (byte)NbtTagType.IntArray:
                    int intArrayLen = NbtStream.ReadInt();
                    int[] ints = new int[intArrayLen];
                    for (int i = 0; i < intArrayLen; i++)
                    {
                        ints[i] = NbtStream.ReadInt();
                    }
                    return CreateValueTag(NbtTagType.IntArray, ints);
                case (byte)NbtTagType.LongArray:
                    long longArrayLen = NbtStream.ReadInt();
                    long[] longs = new long[longArrayLen];
                    for (int i = 0; i < longArrayLen; i++)
                    {
                        longs[i] = NbtStream.ReadLong();
                    }
                    return CreateValueTag(NbtTagType.LongArray, longs);
            }
            throw new InvalidDataException("NBT is invalid.");
        }
    }
    public enum NbtTagType : byte
    {
        Byte = 1,
        Short = 2,
        Int = 3,
        Long = 4,
        Float = 5,
        Double = 6,
        ByteArray = 7,
        String = 8,
        List = 9,
        Compound = 10,
        IntArray = 11,
        LongArray = 12
    }
}
