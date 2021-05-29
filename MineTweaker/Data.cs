using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker
{
    public struct Vec2
    {
        public int X;
        public int Y;
        public Vec2(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public Vec2(float X, float Y)
        {
            this.X = (int)Math.Round(X);
            this.Y = (int)Math.Round(Y);
        }
        public Vec2(float X, float Y, RoundType RoundMethod)
        {
            if (RoundMethod == RoundType.Round)
            {
                this.X = (int)Math.Round(X);
                this.Y = (int)Math.Round(Y);
            }
            else if (RoundMethod == RoundType.Floor)
            {
                this.X = (int)Math.Floor(X);
                this.Y = (int)Math.Floor(Y);
            }
            else
            {
                this.X = (int)Math.Ceiling(X);
                this.Y = (int)Math.Ceiling(Y);
            }
        }
    }
    public struct Vec2F
    {
        public float X;
        public float Y;
        public Vec2F(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public Vec2F(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
    public struct Vec3
    {
        public int X;
        public int Y;
        public int Z;
        public Vec3(int X, int Y, int Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
        public Vec3(float X, float Y, float Z)
        {
            this.X = (int)Math.Round(X);
            this.Y = (int)Math.Round(Y);
            this.Z = (int)Math.Round(Z);
        }
        public Vec3(float X, float Y, float Z, RoundType RoundMethod)
        {
            if (RoundMethod == RoundType.Round)
            {
                this.X = (int)Math.Round(X);
                this.Y = (int)Math.Round(Y);
                this.Z = (int)Math.Round(Z);
            }
            else if (RoundMethod == RoundType.Floor)
            {
                this.X = (int)Math.Floor(X);
                this.Y = (int)Math.Floor(Y);
                this.Z = (int)Math.Floor(Z);
            }
            else
            {
                this.X = (int)Math.Ceiling(X);
                this.Y = (int)Math.Ceiling(Y);
                this.Z = (int)Math.Ceiling(Z);
            }
        }
    }
    public struct Vec3F
    {
        public float X;
        public float Y;
        public float Z;
        public Vec3F(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
        public Vec3F(int X, int Y, int Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }
    public struct Vec3D
    {
        public double X;
        public double Y;
        public double Z;
        public Vec3D(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
        public Vec3D(int X, int Y, int Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }
    public enum RoundType : byte
    {
        Round,
        Floor,
        Ceiling
    }
    public struct Angle
    {
        public byte Steps;
        public float Degrees
        {
            get
            {
                return ((float)Steps / 255F) * 360F;
            }
            set
            {
                if (value > 360F || value < 0)
                {
                    throw new ArgumentException("Must be between 0 and 360");
                }
                Steps = (byte)Math.Round(((value / 360F) * 255));
            }
        }
        public Angle(byte Steps)
        {
            this.Steps = Steps;
        }
        public Angle(float Degrees)
        {
            this.Steps = 0;
            this.Degrees = Degrees;
        }
    }
    public struct EntityPosition
    {
        public Vec3F Location;
        public Angle Yaw;
        public Angle HeadPitch;
        public Angle HeadYaw;
        public bool OnGround;
    }
    public struct Slot
    {
        public bool Occupied;
        public int ItemID;
        public byte Count;
        public NbtTag NBT;
        public Slot(Stream Stream)
        {
            Occupied = Stream.ReadBool();
            if (Occupied)
            {
                ItemID = Stream.ReadVarInt();
                Count = (byte)Stream.ReadByte();
                NBT = NbtTag.FromStream(Stream);
            } else
            {
                ItemID = 0;
                Count = 0;
                NBT = null;
            }
        }
    }
}
