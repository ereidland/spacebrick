﻿//
// BrickChunk.cs
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
    public class TreeBasedBrickChunk
    {
        public const int ChunkSize = 16;
        public const int ChunkSizeMask = 15;
        public const int ChunkSizeShift = 4;

        public static Vector3i GetWorldPosition(Vector3i chunkPosition)
        {
            return new Vector3i(chunkPosition.x << ChunkSizeShift, chunkPosition.y << ChunkSizeShift, chunkPosition.z << ChunkSizeShift);
        }

        public static Vector3i GetChunkPosition(Vector3i worldPosition)
        {
            return new Vector3i(worldPosition.x >> ChunkSizeShift, worldPosition.y >> ChunkSizeShift, worldPosition.z >> ChunkSizeShift);
        }

        public static Vector3i GetChunkLocalPosition(Vector3i worldPosition)
        {
            return new Vector3i(worldPosition.x & ChunkSizeMask, worldPosition.y & ChunkSizeMask, worldPosition.z & ChunkSizeMask);
        }

        private KeyValueTree<BitRect, BrickInfo> _bricks = new KeyValueTree<BitRect, BrickInfo>();

        private bool Contains(int x, int y, int z)
        {
            return x >= 0 && y >= 0 && z >= 0 && x < ChunkSize && y < ChunkSize && z < ChunkSize;
        }

        public Brick GetBrick(int x, int y, int z)
        {
            if (Contains(x, y, z))
            {
                KeyValueTree<BitRect, BrickInfo>.Leaf brickLeaf;
                _bricks.TryGetLeaf(new BitRect(x, y, z, 1, 1, 1), out brickLeaf);
                return new Brick(brickLeaf.Key, brickLeaf.Value);
            }
            return new Brick();
        }

        public void SetBrick(Brick brick) { _bricks.Set(brick.Rect, brick.Info); }

        public IEnumerable<Brick> Bricks
        {
            get
            {
                foreach (var leaf in _bricks.RawLeaves)
                    yield return new Brick(leaf.Key, leaf.Value);
            }
        }

        public int BrickCount { get{ return _bricks.Count; } }

        public Vector3i ChunkPosition { get; private set; }
        public Vector3i WorldPosition { get { return GetWorldPosition(ChunkPosition); } }

        public bool TryGetBrick(BitRect rect, out Brick brick)
        {
            KeyValueTree<BitRect, BrickInfo>.Leaf leaf;
            if (_bricks.TryGetLeaf(rect, out leaf))
            {
                brick = new Brick(leaf.Key, leaf.Value);
                return true;
            }
            brick = new Brick();
            return false;
        }

        public TreeBasedBrickChunk(Vector3i chunkPosition)
        {
            ChunkPosition = chunkPosition;
        }
    }
}

