//
// BitRect.cs
//
// Author:
//       Evan Reidland <er@evanreidland.com>
//
// Copyright (c) 2014 Evan Reidland
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;

namespace Spacebrick
{
    public struct BitRect : IComparable<BitRect>
    {
        public static byte GetFirstFour(byte b) { return (byte)(b >> 4); }
        public static byte GetSecondFour(byte b) { return (byte)(b & 0xF); }

        public static byte SetFirstFour(byte b, byte value)
        {
            //Mask with 00001111 then OR value shifted left 4.
            return (byte)((b & 0xF) | (value << 4));
        }

        public static byte SetSecondFour(byte b, byte value)
        {
            //Mask with 11110000 then OR value masked with 00001111
            return (b & 0xF0) | (value & 0xF);
        }

        private byte _a, _b, _c;

        public byte X
        {
            get { return GetFirstFour(_a); }
            set { _a = SetFirstFour(_a, value); }
        }

        public byte Y
        {
            get { return GetSecondFour(_a); }
            set { _a = SetSecondFour(_a, value); }
        }

        public byte Z
        {
            get { return GetFirstFour(_b); }
            set { _b = SetFirstFour(_b, value); }
        }

        public byte Width
        {
            get { return GetSecondFour(_b); }
            set { _b = SetSecondFour(_b, value); }
        }

        public byte Height
        {
            get { return GetFirstFour(_c); }
            set { _c = SetFirstFour(_c, value); }
        }

        public byte Depth
        {
            get { return GetSecondFour(_c); }
            set { _c = SetSecondFour(_c, value); }
        }

        public bool Contains(int x, int y, int z)
        {
            x -= X;
            y -= Y;
            z -= Z;
            return x >= 0 && y >= 0 && z >= 0 && x < Width && y < Height && z < Depth;
        }

        public bool Intersects(BitRect other)
        {
            int selfAX, selfAY, selfAZ, selfBX, selfBY, selfBZ;
            int otherAX, otherAY, otherAZ, otherBX, otherBY, otherBZ;

            selfAX = X;
            selfAY = Y;
            selfAZ = Z;

            selfBX = selfAX + Width;
            selfBY = selfAY + Height;
            selfBZ = selfAZ + Depth;


            otherAX = other.X;
            otherAY = other.Y;
            otherAZ = other.Z;

            otherBX = otherAX + other.X;
            otherBY = otherAY + other.Y;
            otherBZ = otherAZ + other.Z;

            return Math.Abs(selfAX + selfBX - otherAX - otherBX) < (selfBX - selfAX + otherBX - otherAX)
                && Math.Abs(selfAY + selfBY - otherAY - otherBY) < (selfBY - selfAY + otherBY - otherAY)
                && Math.Abs(selfAZ + selfBZ - otherAZ - otherBZ) < (selfBZ - selfAZ + otherBZ - otherAZ);
        }

        //Used for mapping in a tree.
        public int CompareTo(BitRect other)
        {
            if (Intersects(other))
                return 0;

            if (X < other.X || Y < other.Y || Z < other.Z)
                return -1;

            return 1;
        }

        public BitRect(byte a, byte b, byte c)
        {
            _a = a;
            _b = b;
            _c = c;
        }

        public BitRect(BitRect other)
        {
            this(other._a, other._b, other._c);
        }

        public BitRect(byte x, byte y, byte z, byte width, byte height, byte depth)
        {
            _a = _b = _c = 0;
            X = x;
            Y = y;
            Z = z;
            Width = width;
            Height = height;
            Depth = depth;
        }

        public BitRect(int x, int y, int z, int width, int height, int depth)
        {
            this((byte)x, (byte)y, (byte)z, (byte)width, (byte)height, (byte)depth);
        }
    }
}

