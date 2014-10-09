//
// Voxel.cs
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
    public struct Voxel
    {
        public const ushort IDMask = 0x3FF; //0000 0011 1111 1111

        public const ushort DirectionMask = 0x7000; //0111 0000 0000 0000
        public const ushort DirectionShift = 12;

        public const ushort MetaMask = 0x7C00; //0111 1100 0000 0000
        public const ushort MetaShift = 10;

        public const ushort PointerBoolMask = 0x8000; //1000 0000 0000 0000;
        public const short PointerMask = 0xFFF; //0000 1111 1111 1111

        //Might cram in meta value into _data later.
        private ushort _data;

        public ushort ID
        {
            get { return (ushort)(_data & IDMask); }
            set { _data = (ushort)((_data & ~IDMask) | (value & IDMask)); }
        }

        /// <summary>
        /// Uses bits 1 through 3 to store direction (3 bits)
        /// </summary>
        public BrickDirection Direction
        {
            get { return (BrickDirection)((_data & DirectionMask) >> DirectionShift); }
            set { _data = (ushort)((_data & ~DirectionMask) | ((byte)value << DirectionShift)); }
        }

        /// <summary>
        /// Bits from range 1 through 6 (5 bits)
        /// </summary>
        public byte Meta
        {
            get { return (byte)((_data & MetaMask) >> MetaShift); }
            set { _data = (ushort)((_data & ~MetaMask) | (value << MetaShift)); }
        }

        public ushort Pointer
        {
            get { return (ushort)(_data & PointerMask); }
            set { _data = (ushort)((_data & ~PointerMask) | (value & PointerMask)); }
        }

        public bool IsPointer
        {
            get { return (_data & PointerBoolMask) != 0; }
            set
            {
                _data = (ushort)(_data & ~PointerBoolMask);
                if (value)
                    _data |= PointerBoolMask;
            }
        }

        public bool IsEmpty { get { return ID == 0; } }

        public BlockTypeInfo TypeInfo { get { return BlockTypeInfo.GetTypeInfo(ID); } }

        public Voxel(ushort id, BrickDirection direction = BrickDirection.Forward)
        {
            _data = 0;

            ID = id;
            Direction = direction;
        }

        public Voxel(Voxel other)
        {
            _data = other._data;
        }
    }
}

