//
// VoxelMap.cs
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
    public class VoxelMap
    {
        public const ulong OffsetToPositive = 1000000;
        public const ulong MaxSize = OffsetToPositive*2;

        private static Dictionary<ulong, VoxelChunk> _voxelMap = new Dictionary<ulong, VoxelChunk>();

        private static ulong ConvertChunkPositionToKey(int chunkX, int chunkY, int chunkZ)
        {
            ulong x = (ulong)chunkX + OffsetToPositive;
            ulong y = (ulong)chunkY + OffsetToPositive;
            ulong z = (ulong)chunkZ + OffsetToPositive;

            return z*MaxSize*MaxSize + y*MaxSize + x*MaxSize;
        }

        public VoxelChunk GetChunk(int chunkX, int chunkY, int chunkZ)
        {
            ulong key = ConvertChunkPositionToKey(chunkX, chunkY, chunkZ);
            VoxelChunk chunk;
            _voxelMap.TryGetValue(key, out chunk);
            return chunk;
        }

        public VoxelChunk GetOrCreateChunk(int chunkX, int chunkY, int chunkZ)
        {
            ulong key = ConvertChunkPositionToKey(chunkX, chunkY, chunkZ);
            VoxelChunk chunk;
            if (!_voxelMap.TryGetValue(key, out chunk))
            {
                chunk = new VoxelChunk(this, new Vector3i(chunkX, chunkY, chunkZ));
                _voxelMap[key] = chunk;
            }
            return chunk;
        }

        public Voxel ReadVoxel(int x, int y, int z)
        {
            int chunkX = x, chunkY = y, chunkZ = z;
            VoxelChunk.ConvertWorldToChunk(ref chunkX, ref chunkY, ref chunkZ);
            var chunk = GetChunk(chunkX, chunkY, chunkZ);
            if (chunk != null)
            {
                VoxelChunk.ConvertWorldToLocal(ref x, ref y, ref z);
                return chunk.ReadVoxel(x, y, z);
            }

            return new Voxel();
        }

        public void ReadBlock(BlockQuery block, int x, int y, int z)
        {
            var voxel = ReadVoxel(x, y, z);

            block.Set(voxel, x, y, z);
        }

        public void WriteBlock(BlockTypeInfo typeInfo)
        {
        }
    }
}

