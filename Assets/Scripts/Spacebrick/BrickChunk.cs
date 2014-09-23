﻿//
// VoxelBrickChunk.cs
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
using System.Collections.Generic;

namespace Spacebrick
{
    public enum VoxelIndexMeta : byte
    {
        Empty = 0,
        Here = 1,
        Left = 2,
        Back = 3,
        Down = 4,
        LeftDownBack = 5
    }
    public struct VoxelIndex
    {
        public const int MetaMask = 0xF000; // 1111 0000 0000 0000
        public const int MetaShift = 12;

        public const int IndexMask = 0xFFF; // 0000 1111 1111 1111

        public ushort RawData;
        public ushort Index
        {
            get { return (ushort)(Index & IndexMask); }
            set { RawData = (ushort)((Index & MetaMask) | (value & IndexMask)); }
        }

        public byte Meta
        {
            get { return (byte)(RawData >> MetaShift); }
            set { RawData = (ushort)((value << MetaShift) | Index); }
        }

        public VoxelIndexMeta IndexMeta { get { return (VoxelIndexMeta)Meta; } }
    }

    public class BrickChunk
    {
        public const int ChunkSize = 16;
        public const int ChunkSizeMask = 15;
        public const int ChunkSizeShift = 4;
        public const int ChunkArrayLength = ChunkSize*ChunkSize*ChunkSize;


        public static Vector3i GetWorldPosition(Vector3i chunkPosition)
        {
            return new Vector3i(chunkPosition.x << ChunkSizeShift, chunkPosition.y << ChunkSizeShift, chunkPosition.z << ChunkSizeShift);
        }

        public static Vector3i GetChunkPosition(Vector3i worldPosition)
        {
            return new Vector3i(worldPosition.x >> ChunkSizeShift, worldPosition.y >> ChunkSizeShift, worldPosition.z >> ChunkSizeShift);
        }

        public static void GetChunkLocalPosition(ref int x, ref int y, ref int z)
        {
            x &= ChunkSizeMask;
            y &= ChunkSizeMask;
            z &= ChunkSizeMask;
        }

        public static Vector3i GetChunkLocalPosition(Vector3i worldPosition)
        {
            return new Vector3i(worldPosition.x & ChunkSizeMask, worldPosition.y & ChunkSizeMask, worldPosition.z & ChunkSizeMask);
        }

        private bool Contains(int x, int y, int z)
        {
            return x >= 0 && y >= 0 && z >= 0 && x < ChunkSize && y < ChunkSize && z < ChunkSize;
        }


        private VoxelIndex[] _indexes = new VoxelIndex[ChunkArrayLength];

        //TODO: Trimming down of _bricks list before saving/loading.
        private List<Brick> _bricks = new List<Brick>();

        private int FindSlotForBrick()
        {
            //Check for empty bricks in list.
            for (int i = 0; i < _bricks.Count; i++)
            {
                if (_bricks[i].IsEmpty)
                    return i;
            }

            //No empty bricks? Add one to the end.
            _bricks.Add(new Brick());
            return _bricks.Count - 1;
        }

        public static int GetIndexFromPosition(int x, int y, int z) { return z*ChunkSize*ChunkSize + y*ChunkSize + x; }
        public VoxelIndex GetVoxelIndex(int x, int y, int z) { return _indexes[GetIndexFromPosition(x, y, z)]; }

        public Brick GetBrickAtIndex(int index)
        {
            if (index >= 0 && index < _bricks.Count)
                return _bricks[index];

            return new Brick();
        }

        public void SetBrickAtIndex(int index, Brick brick)
        {
            if (index >= 0 && index < _bricks.Count)
                _bricks[index] = brick;
        }
    }
}

