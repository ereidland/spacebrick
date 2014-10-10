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
using System.Collections.Generic;

namespace Spacebrick
{
    public enum VoxelModification
    {
        Created,
        Removed,
    }

    public class VoxelModifiedEvent
    {
        public VoxelChunk Chunk { get; private set; }
        public VoxelModification Modification { get; private set; }
        public Vector3i LocalPosition { get; private set; }

        public VoxelModifiedEvent(VoxelChunk chunk, VoxelModification modification, Vector3i localPosition)
        {
            Chunk = chunk;
            Modification = modification;
            LocalPosition = localPosition;
        }
    }

    public class VoxelChunk
    {
        public class IndexedVoxel
        {
            public Vector3i Position;
            public Voxel Voxel;
        }

        public const int ChunkSize = 16;
        public const int ChunkSizeShift = 4;
        public const int ChunkSizeMask = ChunkSize - 1;

        public static int GetIndex(int x, int y, int z) { return z*ChunkSize*ChunkSize + y*ChunkSize + x; }
        public static void GetPosition(int index, ref int x, ref int y, ref int z)
        {
            //Not tested.
            z = index/(ChunkSize*ChunkSize);
            y = (index - z*ChunkSize*ChunkSize)/ChunkSize;
            x = index - z*ChunkSize*ChunkSize - y*ChunkSize;
        }
        public static bool ContainsLocal(int x, int y, int z) { return x >= 0 && y >= 0 && z >= 0 && x < ChunkSize && y < ChunkSize && z < ChunkSize; }

        private EventCallbackList _voxelModifiedList;

        private void Notify(VoxelModification modification, int x, int y, int z)
        {
            _voxelModifiedList.Execute(new VoxelModifiedEvent(this, modification, new Vector3i(x, y, z)));
        }

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

        public VoxelMap Map { get; private set; }

        public IEnumerable<Voxel> Voxels
        {
            get
            {
                for (int i = 0; i < _voxels.Length; i++)
                {
                    var voxel = _voxels[i];
                    if (!voxel.IsEmpty)
                        yield return voxel;
                }
            }
        }

        /// <summary>
        /// Returns each voxel and its position within the chunk.
        /// Note that it returns the same reference each time, but with different data.
        /// </summary>
        public IEnumerable<IndexedVoxel> VoxelsByPosition
        {
            get
            {
                var indexedVoxel = new IndexedVoxel();

                for (int x = 0; x < ChunkSize; x++)
                {
                    for (int y = 0; y < ChunkSize; y++)
                    {
                        for (int z = 0; z < ChunkSize; z++)
                        {
                            var voxel = _voxels[GetIndex(x, y, z)];
                            if (!voxel.IsEmpty)
                            {
                                indexedVoxel.Position.Set(x, y, z);
                                indexedVoxel.Voxel = voxel;

                                yield return indexedVoxel;
                            }
                        }
                    }
                }
            }
        }

        public Vector3i ChunkPosition { get { return _chunkPosition; } }
        public Vector3i WorldPosition
        {
            get 
            {
                Vector3i worldPosition = ChunkPosition;
                ConvertChunkToWorld(ref worldPosition.x, ref worldPosition.y, ref worldPosition.z);
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

        public Voxel GetVoxel(int x, int y, int z)
        {
            if (ContainsLocal(x, y, z))
                return _voxels[GetIndex(x, y, z)];

            return new Voxel();
        }

        public Voxel GetVoxelAtIndex(int index)
        {
            if (index >= 0 && index < _voxels.Length)
                return _voxels[index];
            return new Voxel();
        }

        public void SetVoxel(int x, int y, int z, Voxel voxel, bool notify = true)
        {
            if (ContainsLocal(x, y, z))
            {
                _voxels[GetIndex(x, y, z)] = voxel;
                if (notify)
                    Notify(VoxelModification.Created, x, y, z);
            }
        }

        public void SetVoxelAtIndex(int index, Voxel voxel, bool notify = true)
        {
            if (index >= 0 && index < _voxels.Length)
            {
                _voxels[index] = voxel;

                if (notify)
                {
                    int x = 0, y = 0, z = 0;
                    GetPosition(index, ref x, ref y, ref z);
                    Notify(VoxelModification.Created, x, y, z);
                }
            }
        }

        public VoxelChunk(VoxelMap map, Vector3i chunkPosition)
        {
            Map = map;
            _chunkPosition = chunkPosition;
            _voxelModifiedList = map.Events.GetList(typeof(VoxelModifiedEvent));
        }
    }
}

