//
// Brick.cs
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
    public enum BrickDirection : byte
    {
        Forward = 0,
        Right = 1,
        Back = 2,
        Left = 3,
        Up = 4,
        Down = 5
    }

    public struct BrickInfo
    {
        public const ushort IDMask = 0x3FF; //0000 0011 1111 1111

        public const ushort DirectionMask = 0x1C00; //0001 1100 0000 0000
        public const ushort DirectionShift = 10;

        public const ushort MetaMask = 0xFC00; //1111 1100 0000 0000
        public const ushort MetaShift = 10;

        //Might cram in meta value into _data later.
        private ushort _data;

        public ushort ID
        {
            get { return (ushort)(_data & IDMask); }
            set { _data = (ushort)((_data & MetaMask) | (value & IDMask)); }
        }

        /// <summary>
        /// Uses last 3 bits, but not first 3 bits, to store direction.
        /// </summary>
        public BrickDirection Direction
        {
            get { return (BrickDirection)((_data & DirectionMask) >> DirectionShift); }
            set { _data = (ushort)((_data & ~DirectionMask) | ((byte)value << DirectionShift)); }
        }

        /// <summary>
        /// Full 6 bit meta.
        /// </summary>
        public byte Meta
        {
            get { return (byte)((_data & MetaMask) >> MetaShift); }
            set { _data = (ushort)(ID | (value << MetaShift)); }
        }

        public BrickTypeInfo TypeInfo { get { return BrickTypeInfo.GetTypeInfo(ID); } }

        public BrickInfo(ushort id, BrickDirection direction = BrickDirection.Forward)
        {
            _data = 0;

            ID = id;
            Direction = direction;
        }

        public BrickInfo(BrickInfo other)
        {
            _data = other._data;
        }
    }

    public struct Brick
    {
        public BitRect Rect;
        public BrickInfo Info;

        public BrickTypeInfo TypeInfo { get { return BrickTypeInfo.GetTypeInfo(Info.ID); } }

        public bool IsEmpty { get { return Info.ID == 0; } }

        public Brick(BitRect rect, BrickInfo info)
        {
            Rect = rect;
            Info = info;
        }
    }
}

