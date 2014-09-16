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
    public enum BlockDireciton : byte
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
        public const int DataMask = 0x3FF; //0000 0011 1111 1111

        public const int DirectionMask = 0x1C00; //0001 1100 0000 0000
        public const int DirectionShift = 10;

        public const int MetaMask = 0xFC00; //1111 1100 0000 0000
        public const int MetaShift = 10;

        //Might cram in meta value into _data later.
        private short _data;

        public short ID { get { return (short)(_data & DataMask); } }

        /// <summary>
        /// Uses last 3 bits, but not first 3 bits, to store direction.
        /// </summary>
        public BlockDireciton Direction
        {
            get { return (BlockDireciton)((_data & DirectionMask) >> DirectionShift); }
            set { _data = (short)((_data & ~DirectionMask) | ((byte)value << DirectionShift)); }
        }

        /// <summary>
        /// Full 6 bit meta.
        /// </summary>
        public byte Meta
        {
            get { return (byte)((_data & MetaMask) >> MetaShift); }
            set { _data = (short)(ID | (value << MetaShift)); }
        }

        public BrickInfo(short data)
        {
            _data = data;
        }
    }

    public struct Brick
    {
        public BitRect Rect;
        public BrickInfo Info;

        public Brick(BitRect rect, BrickInfo info)
        {
            Rect = rect;
            Info = info;
        }
    }
}

