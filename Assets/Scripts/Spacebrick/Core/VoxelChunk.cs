//
// VoxelChunk.cs
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
    public class VoxelChunk
    {
        public const int ChunkSize = 16;
        public const int ChunkSizeShift = 4;
        public const int ChunkSizeMask = ChunkSize - 1;

        public static int GetIndex(int x, int y, int z) { return z*ChunkSize*ChunkSize + y*ChunkSize + x; }
        public static bool ContainsLocal(int x, int y, int z) { return x >= 0 && y >= 0 && z >= 0 && x < ChunkSize && y < ChunkSize && z < ChunkSize; }

        /// <summary>
        /// Converts a map position to a local position inside of a chunk.
        /// </summary>
        public static void ConvertWorldToLocal(ref int x, ref int y, ref int z)
        {
            x &= ChunkSizeMask;
            y &= ChunkSizeMask;
            z &= ChunkSizeMask;
        }

        /// <summary>
        /// Converts a map position to a local position inside of a chunk.
        /// </summary>
        public static void ConvertWorldToLocal(ref Vector3i pos) { ConvertWorldToLocal(ref pos.x, ref pos.y, ref pos.z); }

        /// <summary>
        /// Converts a map position to a chunk coordinate (not a local chunk position).
        /// </summary>
        public static void ConvertWorldToChunk(ref int x, ref int y, ref int z)
        {
            x >>= ChunkSizeShift;
            y >>= ChunkSizeShift;
            z >>= ChunkSizeShift;
        }

        /// <summary>
        /// Converts a map position to a chunk coordinate (not a local chunk position).
        /// </summary>
        public static void ConvertWorldToChunk(Vector3i pos) { ConvertWorldToChunk(ref pos.x, ref pos.y, ref pos.z); }

        /// <summary>
        /// Converts a chunk coordinate to a map position.
        /// </summary>
        public static void ConvertChunkToWorld(ref int x, ref int y, ref int z)
        {
            x <<= ChunkSizeShift;
            y <<= ChunkSizeShift;
            z <<= ChunkSizeShift;
        }

        /// <summary>
        /// Converts a chunk coordinate to a map position.
        /// </summary>
        public static void ConvertChunkToWorld(ref Vector3i pos) { ConvertChunkToWorld(ref pos.x, ref pos.y, ref pos.z); }

        private Voxel[] _voxels = new Voxel[ChunkSize*ChunkSize*ChunkSize];
        private Vector3i _chunkPosition;

        public VoxelMap Map { get; private set ;}

        public Vector3i ChunkPosition { get { return _chunkPosition; } }
        public Vector3i WorldPosition
        {
            get 
            {
                Vector3i worldPosition = ChunkPosition;
                ConvertLocalToWorld(ref worldPosition.x, ref worldPosition.y, ref worldPosition.z);
                return worldPosition;
            }
        }

        public void ConvertLocalToWorld(ref int x, ref int y, ref int z)
        {
            x += _chunkPosition.x*ChunkSize;
            y += _chunkPosition.y*ChunkSize;
            z += _chunkPosition.z*ChunkSize;
        }

        public void ConvertLocalToWorld(ref Vector3i pos) { ConvertLocalToWorld(ref pos.x, ref pos.y, ref pos.z); }

        public Voxel ReadVoxel(int x, int y, int z)
        {
            if (ContainsLocal(x, y, z))
                return _voxels[GetIndex(x, y, z)];

            return new Voxel();
        }

        public Voxel ReadVoxelFromIndex(int index)
        {
            if (index >= 0 && index < _voxels.Length)
                return _voxels[index];
            return new Voxel();
        }

        public void WriteVoxel(int x, int y, int z, Voxel voxel)
        {
            if (ContainsLocal(x, y, z))
                _voxels[GetIndex(x, y, z)] = voxel;
        }

        public void WriteVoxelAtIndex(int index, Voxel voxel)
        {
            if (index >= 0 && index < _voxels.Length)
                _voxels[index] = voxel;
        }

        public VoxelChunk(VoxelMap map, Vector3i chunkPosition)
        {
            Map = map;
            _chunkPosition = chunkPosition;
        }
    }
}

