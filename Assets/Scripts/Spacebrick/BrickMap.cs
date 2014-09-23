//
// BrickMap.cs
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
    public class BrickMap
    {
        private KeyValueTree<Vector3i, BrickChunk> _chunks = new KeyValueTree<Vector3i, BrickChunk>();

        public BrickChunk GetChunk(Vector3i position)
        {
            BrickChunk chunk;
            _chunks.TryGet(position, out chunk);
            return chunk;
        }

        public BrickChunk GetOrCreateChunk(Vector3i position)
        {
            var chunk = GetChunk(position);
            if (chunk == null)
            {
                chunk = new BrickChunk();
                _chunks.Set(position, chunk);
            }

            return chunk;
        }

        public Brick GetBrick(int x, int y, int z)
        {
            Vector3i chunkPosition = BrickChunk.GetChunkPosition(new Vector3i(x, y, z));
            BrickChunk chunk = GetChunk(chunkPosition);

            if (chunk != null)
            {
                BrickChunk.GetChunkLocalPosition(ref x, ref y, ref z);
                VoxelIndex voxelIndex = chunk.GetVoxelIndex(x, y, z);
                var indexMeta = voxelIndex.IndexMeta;

                switch (indexMeta)
                {
                    case VoxelIndexMeta.Here:
                        //Got it on the first pass!
                        return chunk.GetBrickAtIndex(voxelIndex.Index);
                    case VoxelIndexMeta.Left:
                        chunkPosition.x--;
                        break;
                    case VoxelIndexMeta.Down:
                        chunkPosition.y--;
                        break;
                    case VoxelIndexMeta.Back:
                        chunkPosition.z--;
                        break;
                    case VoxelIndexMeta.LeftDownBack:
                        chunkPosition.x--;
                        chunkPosition.y--;
                        chunkPosition.z--;
                        break;
                }

                //We need to go to another chunk for our data.
                chunk = GetChunk(chunkPosition);
                if (chunk != null)
                    return chunk.GetBrickAtIndex(voxelIndex.Index);
            }

            return new Brick();
        }
    }
}

